using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RackBuilderMod;

public class RackBuilderCore : MelonMod
{
	public struct ItemChoice
	{
		public string name;

		public string category;

		public int prefabIndex;

		public int sizeInU;
	}

	[Serializable]
	public class CableLinkData
	{
		[JsonPropertyName("serverID")]
		public string ServerID { get; set; }

		[JsonPropertyName("serverRackPositionUID")]
		public int ServerRackPositionUID { get; set; }

		[JsonPropertyName("serverPortIndex")]
		public int ServerPortIndex { get; set; }

		[JsonPropertyName("switchID")]
		public string SwitchID { get; set; }

		[JsonPropertyName("switchRackPositionUID")]
		public int SwitchRackPositionUID { get; set; }

		[JsonPropertyName("switchPortIndex")]
		public int SwitchPortIndex { get; set; }

		[JsonPropertyName("patchPanelID")]
		public string PatchPanelID { get; set; }

		[JsonPropertyName("patchPanelRackPositionUID")]
		public int PatchPanelRackPositionUID { get; set; }

		[JsonPropertyName("patchNearPortIndex")]
		public int PatchNearPortIndex { get; set; } = -1;

		[JsonPropertyName("patchFarPortIndex")]
		public int PatchFarPortIndex { get; set; } = -1;
	}

	[Serializable]
	public class CableTopologyData
	{
		[JsonPropertyName("layoutFingerprint")]
		public string LayoutFingerprint { get; set; } = "";

		[JsonPropertyName("cables")]
		public List<CableLinkData> Cables { get; set; } = new List<CableLinkData>();
	}

	[Serializable]
	public class RackRoleEntry
	{
		[JsonPropertyName("rackKey")]
		public string RackKey { get; set; }

		[JsonPropertyName("role")]
		public string Role { get; set; }
	}

	[Serializable]
	public class RackRoleData
	{
		[JsonPropertyName("racks")]
		public List<RackRoleEntry> Racks { get; set; } = new List<RackRoleEntry>();
	}
	

	private bool _integrated;
	private bool _startupCableRestoreQueued;

	// Fingerprint of rack instance IDs at last UI open ? used to detect same-scene save reloads
	private HashSet<int> _lastRackIds = new HashSet<int>();

	// Cable IDs created by AutoWire — shielded from the post-placement scrub coroutine.
	private HashSet<int> _autoWireProtectedCableIds = new HashSet<int>();

	private ComputerShop _shop;

	private GameObject _rackScreen;

	private Transform _contentParent;

	private readonly List<GameObject> _uiRows = new List<GameObject>();

	private bool _onDetailPage;

	private Rack _selectedRack;

	private List<Rack> _allRacks = new List<Rack>();

	private RackMount _pendingRemoveMount;

	private bool _pendingBulkClearConfirmation;

	private Dictionary<int, int> _cartQty = new Dictionary<int, int>();

	private List<(int sfpType, float speed, int prefabIdx, string name)> _sfpPrefabInfo;

	private Dictionary<NetworkSwitch, Dictionary<int, float>> _switchTypeSpeedMap = new Dictionary<NetworkSwitch, Dictionary<int, float>>();

	// When true, ShowRackDetail / ShowRackList calls are no-ops (background auto-wire pass).
	private bool _suppressUiUpdates;

	private List<CableLink> _cachedRackRailClips;

	private Rack _cachedClipsRack;

	private List<CableLink> _cachedOverheadClips;

	private List<ItemChoice> _itemChoices = new List<ItemChoice>();

	private bool _disableAutoWireForDebug = false;

	private bool _enableVerboseDiagnostics = false;

	private const string RackRoleServer = "server";

	private const string RackRoleNetwork = "network";

	private bool _rackRolesLoaded;

	private Dictionary<string, string> _rackRolesByKey = new Dictionary<string, string>();

	private void LogPlacementDebugState(string stage, Server srv, NetworkSwitch sw)
	{
		if (!_enableVerboseDiagnostics)
		{
			return;
		}
		try
		{
			if ((UnityEngine.Object)(object)srv != (UnityEngine.Object)null)
			{
				int ids = 0;
				int idsNonNegative = 0;
				int endpoints = 0;
				if (srv.cablelinks != null)
				{
					foreach (CableLink cl in (Il2CppArrayBase<CableLink>)(object)srv.cablelinks)
					{
						if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null)
							continue;
						if (cl.cableIDsOnLink > 0)
							ids++;
						if (cl.cableIDsOnLink >= 0)
							idsNonNegative++;
						if (cl.isStartOrEnd || cl.isEndPoint)
							endpoints++;
					}
				}
				bool connected = false;
				try { connected = srv.IsAnyCableConnected(); } catch { }
				int activeCount = (srv.activeLinks != null) ? srv.activeLinks.Count : -1;
				((MelonBase)this).LoggerInstance.Msg($"[PlacementDebug:{stage}] Server connected={connected} ids>0={ids} ids>=0={idsNonNegative} endpointFlags={endpoints} activeLinks={activeCount} hasInitialized={srv.hasInitialized} isOn={srv.isOn}");
			}
			if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null)
			{
				int ids2 = 0;
				int idsNonNegative2 = 0;
				int endpoints2 = 0;
				if (sw.cableLinkSwitchPorts != null)
				{
					foreach (CableLink cl2 in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
					{
						if ((UnityEngine.Object)(object)cl2 == (UnityEngine.Object)null)
							continue;
						if (cl2.cableIDsOnLink > 0)
							ids2++;
						if (cl2.cableIDsOnLink >= 0)
							idsNonNegative2++;
						if (cl2.isStartOrEnd || cl2.isEndPoint)
							endpoints2++;
					}
				}
				bool connected2 = false;
				try { connected2 = sw.IsAnyCableConnected(); } catch { }
				int tempCount = (sw.temporarilyDisconnectedCables != null) ? sw.temporarilyDisconnectedCables.Count : -1;
				((MelonBase)this).LoggerInstance.Msg($"[PlacementDebug:{stage}] Switch connected={connected2} ids>0={ids2} ids>=0={idsNonNegative2} endpointFlags={endpoints2} tempDisconnected={tempCount} isOn={sw.isOn}");
			}
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Warning("Placement debug log failed: " + ex.Message);
		}
	}

	public override void OnInitializeMelon()
	{
		((MelonBase)this).LoggerInstance.Msg("Rack Builder Mod v1.1.1 initialized!");
	}

	public override void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		FullReset();
	}

	private void FullReset()
	{
		_integrated = false;
		// Destroy mod-created UI to prevent duplicates after same-scene save reload
		if ((UnityEngine.Object)(object)_rackScreen != (UnityEngine.Object)null)
			UnityEngine.Object.Destroy((UnityEngine.Object)(object)_rackScreen);
		_rackScreen = null;
		if ((UnityEngine.Object)(object)_shop != (UnityEngine.Object)null
			&& (UnityEngine.Object)(object)_shop.mainScreen != (UnityEngine.Object)null)
		{
			Transform btnGrid = _shop.mainScreen.transform.Find("Button Grid");
			if ((UnityEngine.Object)(object)btnGrid != (UnityEngine.Object)null)
			{
				Transform oldBtn = btnGrid.Find("Icon Rack Manager bcg");
				if ((UnityEngine.Object)(object)oldBtn != (UnityEngine.Object)null)
					UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)oldBtn).gameObject);
			}
		}
		_uiRows.Clear();
		_shop = null;
		_selectedRack = null;
		_onDetailPage = false;
		_allRacks.Clear();
		_pendingRemoveMount = null;
		_pendingBulkClearConfirmation = false;
		_cartQty.Clear();
		_itemChoices.Clear();
		_cachedRackRailClips = null;
		_cachedClipsRack = null;
		_cachedOverheadClips = null;
		_sfpPrefabInfo = null;
		_switchTypeSpeedMap.Clear();
		_autoWireProtectedCableIds.Clear();
		_suppressUiUpdates = false;
		_startupCableRestoreQueued = false;
	}

	public override void OnUpdate()
	{
		if (!_integrated)
			TryIntegrate();
		TryQueueStartupCableRestore();
	}

	private void TryQueueStartupCableRestore()
	{
		if (_startupCableRestoreQueued)
			return;
		CablePositions cablePositions = UnityEngine.Object.FindObjectOfType<CablePositions>();
		if ((UnityEngine.Object)(object)cablePositions == (UnityEngine.Object)null)
			return;
		int rackCount = 0;
		foreach (Rack rack in UnityEngine.Object.FindObjectsOfType<Rack>())
		{
			if ((UnityEngine.Object)(object)rack != (UnityEngine.Object)null)
				rackCount++;
		}
		if (rackCount == 0)
			return;
		if (!ShouldRestoreFromSavedTopology(out int expectedCableCount, out int liveCableCount))
			return;
		_startupCableRestoreQueued = true;
		((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Startup cable restore queued (live={liveCableCount}, expected={expectedCableCount})");
		MelonCoroutines.Start(RestoreCablesDeferred(10));
	}



	private void CheckForSaveReload()
	{
		// Build current set of Rack instance IDs in the scene
		HashSet<int> currentIds = new HashSet<int>();
		foreach (Rack r in UnityEngine.Object.FindObjectsOfType<Rack>())
		{
			if ((UnityEngine.Object)(object)r != (UnityEngine.Object)null)
				currentIds.Add(((UnityEngine.Object)(object)r).GetInstanceID());
		}

		// First observation only establishes the baseline. Without this, the first time the user
		// opens Rack Manager we misclassify the current scene as a save reload and kick off the
		// full reset/autowire pipeline on the UI open path, which causes the visible freeze.
		if (_lastRackIds.Count == 0)
		{
			_lastRackIds = currentIds;
			return;
		}

		// If the set has meaningfully changed, a save was likely reloaded
		bool changed = currentIds.Count != _lastRackIds.Count
			|| !currentIds.SetEquals(_lastRackIds);

		_lastRackIds = currentIds;

		if (changed && _lastRackIds.Count > 0)
		{
			((MelonBase)this).LoggerInstance.Msg("[RackBuilder] Rack set changed ? resetting state for new save");
			bool wasShowing = _rackScreen != null && _rackScreen.activeSelf;
			FullReset();
			// Re-integrate immediately so the UI is available without waiting
			TryIntegrate();
			if (wasShowing)
				ShowRackList();
			// Restore persisted cables only; full rack-wide autowire on load can stall large saves.
			MelonCoroutines.Start(RestoreCablesDeferred(10));
			return;
		}

		if (ShouldRestoreFromSavedTopology(out int expectedCableCount, out int liveCableCount))
		{
			((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Topology mismatch detected after menu reload (live={liveCableCount}, expected={expectedCableCount})");
			MelonCoroutines.Start(RestoreCablesDeferred(10));
		}
	}

	private IEnumerator RestoreCablesDeferred(int frames)
	{
		for (int i = 0; i < frames; i++)
			yield return null;
		((MelonBase)this).LoggerInstance.Msg("[RackBuilder] Restore-only cable pass starting…");
		ClearOrphanedCableIds();
		RestoreCableTopologyFromFile();
		ClearEmptySfpPortSpeeds();
		SaveCableTopology();
		((MelonBase)this).LoggerInstance.Msg("[RackBuilder] Restore-only cable pass complete");
	}

	private IEnumerator EnableCollidersDelayed(GameObject go)
	{
		yield return null;
		yield return null;
		if (!((UnityEngine.Object)(object)go != (UnityEngine.Object)null))
		{
			yield break;
		}
		foreach (Collider col in go.GetComponentsInChildren<Collider>())
		{
			if ((UnityEngine.Object)(object)col != (UnityEngine.Object)null)
			{
				col.enabled = true;
			}
		}
	}

	private IEnumerator RefreshRackDetailDeferred(int frames)
	{
		for (int i = 0; i < frames; i++)
			yield return null;
		if (_onDetailPage && (UnityEngine.Object)(object)_selectedRack != (UnityEngine.Object)null)
			ShowRackDetail();
	}

	private IEnumerator EnforceSfpSpeedDeferred(SFPModule module, CableLink port, float expectedSpeed, int frames)
	{
		for (int i = 0; i < frames; i++)
		{
			yield return null;
			if (expectedSpeed <= 0f) yield break;
			if ((UnityEngine.Object)(object)module != (UnityEngine.Object)null && module.speed != expectedSpeed)
				module.speed = expectedSpeed;
			if ((UnityEngine.Object)(object)port != (UnityEngine.Object)null && port.connectionSpeed != expectedSpeed)
				port.connectionSpeed = expectedSpeed;
		}
	}

	private IEnumerator RefreshPatchPanelDeferred(PatchPanel panel)
	{
		// Intentionally do not call ValidateRackPosition here ? it triggers internal auto-wiring.
		yield break;
	}

	/// <summary>
	/// After a save/reload the game's InsertedInRack resets cable state from save-data that never
	/// included our mod-created cables. This coroutine re-runs AutoWire on every rack so the
	/// visual cables are restored without user interaction.
	/// </summary>
	private IEnumerator AutoWireAllRacksDeferred(int frames)
	{
		for (int i = 0; i < frames; i++) yield return null;
		((MelonBase)this).LoggerInstance.Msg("[RackBuilder] Post-reload auto-wire starting…");
		// Clear stale cableIDsOnLink values that the game saved from a prior session but whose
		// visual cable was never persisted in CablePositions. Without this, ports look occupied
		// and auto-wire skips them on every reload after the first.
		ClearOrphanedCableIds();
		// Restore cables from our persistent topology file before running auto-wire.
		RestoreCableTopologyFromFile();
		Rack savedRack = _selectedRack;
		int wiredCount = 0;
		_suppressUiUpdates = true;
		try
		{
			foreach (Rack r in UnityEngine.Object.FindObjectsOfType<Rack>())
			{
				if ((UnityEngine.Object)(object)r == (UnityEngine.Object)null) continue;
				_selectedRack = r;
				AutowireRackSilent(ref wiredCount);
			}
		}
		finally
		{
			_selectedRack = savedRack;
			_suppressUiUpdates = false;
		}
		((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Post-reload auto-wire complete — {wiredCount} cables created.");
		ClearEmptySfpPortSpeeds();
		// Persist cable topology so it survives next save/reload cycle.
		SaveCableTopology();
	}

	private static bool IsPlacedRackObject(Component component)
	{
		if ((UnityEngine.Object)(object)component == (UnityEngine.Object)null)
			return false;
		UsableObject uo = component.GetComponent<UsableObject>() ?? component.GetComponentInChildren<UsableObject>();
		if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null)
			return false;
		return (UnityEngine.Object)(object)uo.currentRackPosition != (UnityEngine.Object)null || uo.rackPositionUID > 0;
	}

	private static int GetRackPositionUid(Component component)
	{
		if ((UnityEngine.Object)(object)component == (UnityEngine.Object)null)
			return 0;
		UsableObject uo = component.GetComponent<UsableObject>() ?? component.GetComponentInChildren<UsableObject>();
		if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null)
			return 0;
		if ((UnityEngine.Object)(object)uo.currentRackPosition != (UnityEngine.Object)null && uo.currentRackPosition.rackPosGlobalUID > 0)
			return uo.currentRackPosition.rackPosGlobalUID;
		return uo.rackPositionUID;
	}

	private static Rack GetRackFromPort(CableLink port)
	{
		if ((UnityEngine.Object)(object)port == (UnityEngine.Object)null)
			return null;
		Rack fromHierarchy = ((Component)port).GetComponentInParent<Rack>();
		if ((UnityEngine.Object)(object)fromHierarchy != (UnityEngine.Object)null)
			return fromHierarchy;

		UsableObject owner = null;
		if ((UnityEngine.Object)(object)port.parentServer != (UnityEngine.Object)null)
			owner = (UsableObject)(object)port.parentServer;
		else if ((UnityEngine.Object)(object)port.parentSwitch != (UnityEngine.Object)null)
			owner = (UsableObject)(object)port.parentSwitch;
		else if ((UnityEngine.Object)(object)port.parentPatchPanel != (UnityEngine.Object)null)
			owner = (UsableObject)(object)port.parentPatchPanel;

		if ((UnityEngine.Object)(object)owner != (UnityEngine.Object)null && (UnityEngine.Object)(object)owner.currentRackPosition != (UnityEngine.Object)null)
		{
			Rack fromRackPosition = ((Component)owner.currentRackPosition).GetComponentInParent<Rack>();
			if ((UnityEngine.Object)(object)fromRackPosition != (UnityEngine.Object)null)
				return fromRackPosition;
		}

		return null;
	}

	private static Rack ResolveRackForRoute(params CableLink[] ports)
	{
		if (ports == null)
			return null;
		foreach (CableLink port in ports)
		{
			Rack rack = GetRackFromPort(port);
			if ((UnityEngine.Object)(object)rack != (UnityEngine.Object)null)
				return rack;
		}
		return null;
	}

	private static string BuildCurrentTopologyFingerprint()
	{
		List<string> parts = new List<string>();
		foreach (Server server in UnityEngine.Object.FindObjectsOfType<Server>())
		{
			if ((UnityEngine.Object)(object)server == (UnityEngine.Object)null || server.cablelinks == null || !IsPlacedRackObject((Component)(object)server))
				continue;
			int rackPositionUid = GetRackPositionUid((Component)(object)server);
			if (rackPositionUid <= 0)
				continue;
			parts.Add($"S:{rackPositionUid}:{((Il2CppArrayBase<CableLink>)(object)server.cablelinks).Length}");
		}
		foreach (NetworkSwitch networkSwitch in UnityEngine.Object.FindObjectsOfType<NetworkSwitch>())
		{
			if ((UnityEngine.Object)(object)networkSwitch == (UnityEngine.Object)null || networkSwitch.cableLinkSwitchPorts == null || !IsPlacedRackObject((Component)(object)networkSwitch))
				continue;
			int rackPositionUid = GetRackPositionUid((Component)(object)networkSwitch);
			if (rackPositionUid <= 0)
				continue;
			parts.Add($"W:{rackPositionUid}:{((Il2CppArrayBase<CableLink>)(object)networkSwitch.cableLinkSwitchPorts).Length}");
		}
		foreach (PatchPanel patchPanel in UnityEngine.Object.FindObjectsOfType<PatchPanel>())
		{
			if ((UnityEngine.Object)(object)patchPanel == (UnityEngine.Object)null || patchPanel.cableLinkPorts == null || !IsPlacedRackObject((Component)(object)patchPanel))
				continue;
			int rackPositionUid = GetRackPositionUid((Component)(object)patchPanel);
			if (rackPositionUid <= 0)
				continue;
			parts.Add($"P:{rackPositionUid}:{((Il2CppArrayBase<CableLink>)(object)patchPanel.cableLinkPorts).Length}");
		}
		parts.Sort(StringComparer.Ordinal);
		return string.Join("|", parts);
	}

	private static int CountLiveServerPortsWithPersistentCables()
	{
		int count = 0;
		foreach (Server server in UnityEngine.Object.FindObjectsOfType<Server>())
		{
			if ((UnityEngine.Object)(object)server == (UnityEngine.Object)null || server.cablelinks == null || !IsPlacedRackObject((Component)(object)server))
				continue;
			foreach (CableLink port in (Il2CppArrayBase<CableLink>)(object)server.cablelinks)
			{
				if ((UnityEngine.Object)(object)port != (UnityEngine.Object)null && port.cableIDsOnLink > 0)
					count++;
			}
		}
		return count;
	}

	private bool ShouldRestoreFromSavedTopology(out int expectedCableCount, out int liveCableCount)
	{
		expectedCableCount = 0;
		liveCableCount = 0;
		try
		{
			string filePath = Path.Combine(UnityEngine.Application.persistentDataPath, "RackCables.json");
			if (!File.Exists(filePath))
				return false;
			string json = File.ReadAllText(filePath);
			CableTopologyData topology = JsonSerializer.Deserialize<CableTopologyData>(json);
			if (topology == null || topology.Cables == null || topology.Cables.Count == 0)
				return false;
			string currentFingerprint = BuildCurrentTopologyFingerprint();
			if (!string.IsNullOrEmpty(topology.LayoutFingerprint) && !string.Equals(topology.LayoutFingerprint, currentFingerprint, StringComparison.Ordinal))
				return false;
			expectedCableCount = topology.Cables.Count;
			liveCableCount = CountLiveServerPortsWithPersistentCables();
			return liveCableCount < expectedCableCount;
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Warning("[RackBuilder] Persistent cable restore check failed: " + ex.Message);
			return false;
		}
	}

	private static string GetRackRolesFilePath()
	{
		return Path.Combine(UnityEngine.Application.persistentDataPath, "RackRoles.json");
	}

	private string BuildRackRoleKey(Rack rack)
	{
		if ((UnityEngine.Object)(object)rack == (UnityEngine.Object)null || rack.positions == null)
			return "";
		List<int> uids = new List<int>();
		foreach (RackPosition rp in (Il2CppArrayBase<RackPosition>)(object)rack.positions)
		{
			if ((UnityEngine.Object)(object)rp == (UnityEngine.Object)null)
				continue;
			if (rp.rackPosGlobalUID > 0)
				uids.Add(rp.rackPosGlobalUID);
		}
		if (uids.Count == 0)
			return "";
		uids.Sort();
		return string.Join("-", uids);
	}

	private void EnsureRackRolesLoaded()
	{
		if (_rackRolesLoaded)
			return;
		_rackRolesLoaded = true;
		_rackRolesByKey.Clear();
		try
		{
			string path = GetRackRolesFilePath();
			if (!File.Exists(path))
				return;
			string json = File.ReadAllText(path);
			RackRoleData data = JsonSerializer.Deserialize<RackRoleData>(json);
			if (data?.Racks == null)
				return;
			foreach (RackRoleEntry entry in data.Racks)
			{
				if (entry == null || string.IsNullOrEmpty(entry.RackKey) || string.IsNullOrEmpty(entry.Role))
					continue;
				string role = entry.Role.ToLowerInvariant();
				if (role != RackRoleServer && role != RackRoleNetwork)
					continue;
				_rackRolesByKey[entry.RackKey] = role;
			}
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Warning("Failed to load rack roles: " + ex.Message);
		}
	}

	private void SaveRackRoles()
	{
		try
		{
			RackRoleData data = new RackRoleData();
			foreach (KeyValuePair<string, string> kv in _rackRolesByKey)
			{
				data.Racks.Add(new RackRoleEntry { RackKey = kv.Key, Role = kv.Value });
			}
			string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(GetRackRolesFilePath(), json);
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Warning("Failed to save rack roles: " + ex.Message);
		}
	}

	private string GetRackRole(Rack rack)
	{
		EnsureRackRolesLoaded();
		string key = BuildRackRoleKey(rack);
		if (string.IsNullOrEmpty(key))
			return RackRoleServer;
		if (_rackRolesByKey.TryGetValue(key, out string role) && !string.IsNullOrEmpty(role))
			return role;
		return RackRoleServer;
	}

	private void ToggleRackRole(Rack rack)
	{
		EnsureRackRolesLoaded();
		string key = BuildRackRoleKey(rack);
		if (string.IsNullOrEmpty(key))
			return;
		string oldRole = GetRackRole(rack);
		string newRole = oldRole == RackRoleNetwork ? RackRoleServer : RackRoleNetwork;
		_rackRolesByKey[key] = newRole;
		SaveRackRoles();
		((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Rack role set to {newRole} for key {key}");
	}

	private List<UsableObject> CollectRackUsableObjects(Rack rack)
	{
		Rack prev = _selectedRack;
		try
		{
			_selectedRack = rack;
			return CollectRackUsableObjects();
		}
		finally
		{
			_selectedRack = prev;
		}
	}

	private List<NetworkSwitch> CollectRackSwitches(Rack rack)
	{
		return ExtractRackSwitches(CollectRackUsableObjects(rack));
	}

	private List<Server> CollectRackServers(Rack rack)
	{
		return ExtractRackServers(CollectRackUsableObjects(rack));
	}

	private List<Transform> BuildRackEnterPath(CableLink endPort)
	{
		List<Transform> exitPath = BuildRackExitPath(endPort);
		exitPath.Reverse();
		return exitPath;
	}

	private int WireRackSwitchesToCustomer(Rack sourceRack, int targetBaseId)
	{
		if ((UnityEngine.Object)(object)sourceRack == (UnityEngine.Object)null)
			return 0;
		CablePositions cp = UnityEngine.Object.FindObjectOfType<CablePositions>();
		if ((UnityEngine.Object)(object)cp == (UnityEngine.Object)null)
			return 0;
		CustomerBase targetBase = null;
		foreach (CustomerBase candidate in UnityEngine.Object.FindObjectsOfType<CustomerBase>())
		{
			if ((UnityEngine.Object)(object)candidate != (UnityEngine.Object)null && candidate.customerBaseID == targetBaseId)
			{
				targetBase = candidate;
				break;
			}
		}
		if ((UnityEngine.Object)(object)targetBase == (UnityEngine.Object)null || targetBase.cableLinks == null)
			return 0;

		List<CableLink> freeCustomerPorts = new List<CableLink>();
		foreach (CableLink basePort in (Il2CppArrayBase<CableLink>)(object)targetBase.cableLinks)
		{
			if (IsPortReadyForCable(basePort))
				freeCustomerPorts.Add(basePort);
		}
		if (freeCustomerPorts.Count == 0)
			return 0;

		List<CableLink> sourceSwitchPorts = new List<CableLink>();
		foreach (NetworkSwitch sw in CollectRackSwitches(sourceRack))
		{
			if ((UnityEngine.Object)(object)sw == (UnityEngine.Object)null || sw.cableLinkSwitchPorts == null)
				continue;
			foreach (CableLink swPort in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
			{
				if (!IsPortReadyForCable(swPort))
					continue;
				sourceSwitchPorts.Add(swPort);
			}
		}
		if (sourceSwitchPorts.Count == 0)
			return 0;

		Vector3 basePos = ((Component)targetBase).transform.position;
		Vector3 rackPos = ((Component)sourceRack).transform.position;
		List<Transform> overhead = BuildOverheadPath(rackPos, basePos);
		int wired = 0;
		foreach (CableLink swPort in sourceSwitchPorts)
		{
			CableLink basePort = freeCustomerPorts.FirstOrDefault((CableLink p) => IsPortReadyForCable(p) && p.isFibrePort == swPort.isFibrePort);
			if ((UnityEngine.Object)(object)basePort == (UnityEngine.Object)null)
				continue;
			Rack prev = _selectedRack;
			try
			{
				_selectedRack = sourceRack;
				List<Transform> waypoints = new List<Transform>();
				waypoints.AddRange(BuildRackExitPath(swPort));
				waypoints.AddRange(overhead);
				if (CreateCable(cp, swPort, basePort, waypoints, CableLink.TypeOfLink.Switch, CableLink.TypeOfLink.Base, ""))
					wired++;
			}
			finally
			{
				_selectedRack = prev;
			}
		}
		return wired;
	}

	private void AutoWireServerRackToCustomerViaNetwork(int targetBaseId)
	{
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null)
			return;
		Rack serverRack = _selectedRack;
		List<Rack> networkRacks = UnityEngine.Object.FindObjectsOfType<Rack>()
			.Where((Rack r) => (UnityEngine.Object)(object)r != (UnityEngine.Object)null && (UnityEngine.Object)(object)r != (UnityEngine.Object)(object)serverRack && GetRackRole(r) == RackRoleNetwork)
			.ToList();
		if (networkRacks.Count == 0)
		{
			((MelonBase)this).LoggerInstance.Msg("No network racks defined. Toggle at least one rack to NETWORK role.");
			ShowRackDetail();
			return;
		}

		CablePositions cp = UnityEngine.Object.FindObjectOfType<CablePositions>();
		if ((UnityEngine.Object)(object)cp == (UnityEngine.Object)null)
			return;

		Rack targetNetworkRack = networkRacks
			.OrderBy((Rack r) => (((Component)r).transform.position - ((Component)serverRack).transform.position).sqrMagnitude)
			.First();

		List<CableLink> serverRackSwitchPorts = new List<CableLink>();
		foreach (NetworkSwitch sw in CollectRackSwitches(serverRack))
		{
			if ((UnityEngine.Object)(object)sw == (UnityEngine.Object)null || sw.cableLinkSwitchPorts == null)
				continue;
			foreach (CableLink port in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
			{
				if (IsPortReadyForCable(port))
					serverRackSwitchPorts.Add(port);
			}
		}

		List<CableLink> networkRackSwitchPorts = new List<CableLink>();
		foreach (NetworkSwitch sw in CollectRackSwitches(targetNetworkRack))
		{
			if ((UnityEngine.Object)(object)sw == (UnityEngine.Object)null || sw.cableLinkSwitchPorts == null)
				continue;
			foreach (CableLink port in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
			{
				if (IsPortReadyForCable(port))
					networkRackSwitchPorts.Add(port);
			}
		}

		Vector3 from = ((Component)serverRack).transform.position;
		Vector3 to = ((Component)targetNetworkRack).transform.position;
		List<Transform> interRackOverhead = BuildOverheadPath(from, to);
		int trunkCount = 0;
		foreach (CableLink src in serverRackSwitchPorts)
		{
			CableLink dst = networkRackSwitchPorts.FirstOrDefault((CableLink p) => IsPortReadyForCable(p) && p.isFibrePort == src.isFibrePort);
			if ((UnityEngine.Object)(object)dst == (UnityEngine.Object)null)
				continue;
			Rack prev = _selectedRack;
			try
			{
				List<Transform> waypoints = new List<Transform>();
				_selectedRack = serverRack;
				waypoints.AddRange(BuildRackExitPath(src));
				waypoints.AddRange(interRackOverhead);
				_selectedRack = targetNetworkRack;
				waypoints.AddRange(BuildRackEnterPath(dst));
				if (CreateCable(cp, src, dst, waypoints, CableLink.TypeOfLink.Switch, CableLink.TypeOfLink.Switch, ""))
					trunkCount++;
			}
			finally
			{
				_selectedRack = prev;
			}
		}

		int customerCount = WireRackSwitchesToCustomer(targetNetworkRack, targetBaseId);
		((MelonBase)this).LoggerInstance.Msg($"Server->Network->Customer complete: trunks={trunkCount}, customerLinks={customerCount}");
		SaveCableTopology();
		ShowRackDetail();
	}

	/// <summary>
	/// Saves the current cable topology (server→switch mappings) to RackCables.json so it can
	/// be restored after a save/reload cycle. The game doesn't persist our mod-created cables,
	/// so we maintain our own independent cable topology file.
	/// </summary>
	private void SaveCableTopology()
	{
		try
		{
			var topology = new CableTopologyData();
			topology.LayoutFingerprint = BuildCurrentTopologyFingerprint();
			HashSet<string> seen = new HashSet<string>();
			var servers = UnityEngine.Object.FindObjectsOfType<Server>().Where((Server s) => (UnityEngine.Object)(object)s != (UnityEngine.Object)null && s.cablelinks != null && IsPlacedRackObject((Component)(object)s) && !string.IsNullOrEmpty(s.ServerID) && s.ServerID.StartsWith("Mod_")).ToList();
			var switches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>().Where((NetworkSwitch s) => (UnityEngine.Object)(object)s != (UnityEngine.Object)null && s.cableLinkSwitchPorts != null && IsPlacedRackObject((Component)(object)s) && !string.IsNullOrEmpty(s.switchId) && s.switchId.StartsWith("Mod_")).ToList();
			var panels = UnityEngine.Object.FindObjectsOfType<PatchPanel>().Where((PatchPanel p) => (UnityEngine.Object)(object)p != (UnityEngine.Object)null && p.cableLinkPorts != null && IsPlacedRackObject((Component)(object)p) && !string.IsNullOrEmpty(p.patchPanelId) && p.patchPanelId.StartsWith("Mod_")).ToList();

			Dictionary<int, CableLink> switchPortByCableId = new Dictionary<int, CableLink>();
			foreach (NetworkSwitch sw in switches)
			{
				foreach (CableLink swPort in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
				{
					if ((UnityEngine.Object)(object)swPort == (UnityEngine.Object)null) continue;
					int id = swPort.cableIDsOnLink;
					if (id > 0 && !switchPortByCableId.ContainsKey(id))
						switchPortByCableId[id] = swPort;
				}
			}

			Dictionary<int, CableLink> patchPortByCableId = new Dictionary<int, CableLink>();
			foreach (PatchPanel panel in panels)
			{
				foreach (CableLink ppPort in (Il2CppArrayBase<CableLink>)(object)panel.cableLinkPorts)
				{
					if ((UnityEngine.Object)(object)ppPort == (UnityEngine.Object)null) continue;
					int id = ppPort.cableIDsOnLink;
					if (id > 0 && !patchPortByCableId.ContainsKey(id))
						patchPortByCableId[id] = ppPort;
				}
			}

			foreach (Server srv in servers)
			{
				int portIdx = 0;
				foreach (CableLink srvPort in (Il2CppArrayBase<CableLink>)(object)srv.cablelinks)
				{
					if ((UnityEngine.Object)(object)srvPort == (UnityEngine.Object)null)
					{
						portIdx++;
						continue;
					}
					int serverCableId = srvPort.cableIDsOnLink;
					if (serverCableId <= 0)
					{
						portIdx++;
						continue;
					}

					if (switchPortByCableId.TryGetValue(serverCableId, out CableLink directSwitchPort))
					{
						NetworkSwitch sw = directSwitchPort.parentSwitch;
						int swPortIdx = GetSwitchPortIndex(directSwitchPort);
						string key = $"direct:{srv.ServerID}:{portIdx}:{sw.switchId}:{swPortIdx}";
						if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null && swPortIdx >= 0 && seen.Add(key))
						{
							topology.Cables.Add(new CableLinkData
							{
								ServerID = srv.ServerID,
								ServerRackPositionUID = GetRackPositionUid((Component)(object)srv),
								ServerPortIndex = portIdx,
								SwitchID = sw.switchId,
								SwitchRackPositionUID = GetRackPositionUid((Component)(object)sw),
								SwitchPortIndex = swPortIdx
							});
						}
						portIdx++;
						continue;
					}

					if (patchPortByCableId.TryGetValue(serverCableId, out CableLink patchNear) && (UnityEngine.Object)(object)patchNear.parentPatchPanel != (UnityEngine.Object)null)
					{
						PatchPanel panel = patchNear.parentPatchPanel;
						CableLink patchFar = panel.GetPairedLink(patchNear);
						if ((UnityEngine.Object)(object)patchFar != (UnityEngine.Object)null && switchPortByCableId.TryGetValue(patchFar.cableIDsOnLink, out CableLink routedSwitchPort))
						{
							NetworkSwitch sw = routedSwitchPort.parentSwitch;
							int swPortIdx = GetSwitchPortIndex(routedSwitchPort);
							int patchNearIdx = GetPatchPanelPortIndex(patchNear);
							int patchFarIdx = GetPatchPanelPortIndex(patchFar);
							string key = $"patch:{srv.ServerID}:{portIdx}:{panel.patchPanelId}:{patchNearIdx}:{patchFarIdx}:{sw.switchId}:{swPortIdx}";
							if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null && swPortIdx >= 0 && patchNearIdx >= 0 && patchFarIdx >= 0 && seen.Add(key))
							{
								topology.Cables.Add(new CableLinkData
								{
									ServerID = srv.ServerID,
									ServerRackPositionUID = GetRackPositionUid((Component)(object)srv),
									ServerPortIndex = portIdx,
									SwitchID = sw.switchId,
									SwitchRackPositionUID = GetRackPositionUid((Component)(object)sw),
									SwitchPortIndex = swPortIdx,
									PatchPanelID = panel.patchPanelId,
									PatchPanelRackPositionUID = GetRackPositionUid((Component)(object)panel),
									PatchNearPortIndex = patchNearIdx,
									PatchFarPortIndex = patchFarIdx
								});
							}
						}
					}
					portIdx++;
				}
			}
			string filePath = Path.Combine(UnityEngine.Application.persistentDataPath, "RackCables.json");
			string json = JsonSerializer.Serialize(topology, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(filePath, json);
			((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Saved {topology.Cables.Count} cables to RackCables.json (servers={servers.Count}, switches={switches.Count}, patchPanels={panels.Count})");
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Warning($"Failed to save cable topology: {ex.Message}");
		}
	}

	/// <summary>
	/// Restores cable topology from RackCables.json and recreates cables based on the saved mappings.
	/// Called during AutoWireAllRacksDeferred to rebuild cables that survived the save/reload cycle.
	/// </summary>
	private void RestoreCableTopologyFromFile()
	{
		try
		{
			string filePath = Path.Combine(UnityEngine.Application.persistentDataPath, "RackCables.json");
			if (!File.Exists(filePath)) return;
			string json = File.ReadAllText(filePath);
			var topology = JsonSerializer.Deserialize<CableTopologyData>(json);
			if (topology?.Cables.Count == 0) return;
			CablePositions cp = UnityEngine.Object.FindObjectOfType<CablePositions>();
			if ((UnityEngine.Object)(object)cp == (UnityEngine.Object)null) return;
			var serversById = UnityEngine.Object.FindObjectsOfType<Server>()
				.Where((Server s) => (UnityEngine.Object)(object)s != (UnityEngine.Object)null && s.cablelinks != null && IsPlacedRackObject((Component)(object)s) && !string.IsNullOrEmpty(s.ServerID) && s.ServerID.StartsWith("Mod_"))
				.GroupBy((Server s) => s.ServerID)
				.ToDictionary((IGrouping<string, Server> g) => g.Key, (IGrouping<string, Server> g) => g.First());
			var serversByRackPositionUid = UnityEngine.Object.FindObjectsOfType<Server>()
				.Where((Server s) => (UnityEngine.Object)(object)s != (UnityEngine.Object)null && s.cablelinks != null && IsPlacedRackObject((Component)(object)s))
				.Select((Server s) => new { Server = s, RackPositionUid = GetRackPositionUid((Component)(object)s) })
				.Where((x) => x.RackPositionUid > 0)
				.GroupBy((x) => x.RackPositionUid)
				.ToDictionary((IGrouping<int, dynamic> g) => g.Key, (IGrouping<int, dynamic> g) => (Server)g.First().Server);
			var switchesById = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>()
				.Where((NetworkSwitch s) => (UnityEngine.Object)(object)s != (UnityEngine.Object)null && s.cableLinkSwitchPorts != null && IsPlacedRackObject((Component)(object)s) && !string.IsNullOrEmpty(s.switchId) && s.switchId.StartsWith("Mod_"))
				.GroupBy((NetworkSwitch s) => s.switchId)
				.ToDictionary((IGrouping<string, NetworkSwitch> g) => g.Key, (IGrouping<string, NetworkSwitch> g) => g.First());
			var switchesByRackPositionUid = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>()
				.Where((NetworkSwitch s) => (UnityEngine.Object)(object)s != (UnityEngine.Object)null && s.cableLinkSwitchPorts != null && IsPlacedRackObject((Component)(object)s))
				.Select((NetworkSwitch s) => new { Switch = s, RackPositionUid = GetRackPositionUid((Component)(object)s) })
				.Where((x) => x.RackPositionUid > 0)
				.GroupBy((x) => x.RackPositionUid)
				.ToDictionary((IGrouping<int, dynamic> g) => g.Key, (IGrouping<int, dynamic> g) => (NetworkSwitch)g.First().Switch);
			var panelsById = UnityEngine.Object.FindObjectsOfType<PatchPanel>()
				.Where((PatchPanel p) => (UnityEngine.Object)(object)p != (UnityEngine.Object)null && p.cableLinkPorts != null && IsPlacedRackObject((Component)(object)p) && !string.IsNullOrEmpty(p.patchPanelId) && p.patchPanelId.StartsWith("Mod_"))
				.GroupBy((PatchPanel p) => p.patchPanelId)
				.ToDictionary((IGrouping<string, PatchPanel> g) => g.Key, (IGrouping<string, PatchPanel> g) => g.First());
			var panelsByRackPositionUid = UnityEngine.Object.FindObjectsOfType<PatchPanel>()
				.Where((PatchPanel p) => (UnityEngine.Object)(object)p != (UnityEngine.Object)null && p.cableLinkPorts != null && IsPlacedRackObject((Component)(object)p))
				.Select((PatchPanel p) => new { Panel = p, RackPositionUid = GetRackPositionUid((Component)(object)p) })
				.Where((x) => x.RackPositionUid > 0)
				.GroupBy((x) => x.RackPositionUid)
				.ToDictionary((IGrouping<int, dynamic> g) => g.Key, (IGrouping<int, dynamic> g) => (PatchPanel)g.First().Panel);
			int restored = 0;
			int restoredByRackPositionFallback = 0;
			foreach (var cable in topology.Cables)
			{
				Server srv = null;
				NetworkSwitch sw = null;
				PatchPanel panel = null;

				bool resolvedServerById = !string.IsNullOrEmpty(cable.ServerID) && serversById.TryGetValue(cable.ServerID, out srv);
				if (!resolvedServerById && cable.ServerRackPositionUID > 0)
				{
					serversByRackPositionUid.TryGetValue(cable.ServerRackPositionUID, out srv);
					resolvedServerById = (UnityEngine.Object)(object)srv != (UnityEngine.Object)null;
				}
				bool resolvedSwitchById = !string.IsNullOrEmpty(cable.SwitchID) && switchesById.TryGetValue(cable.SwitchID, out sw);
				if (!resolvedSwitchById && cable.SwitchRackPositionUID > 0)
				{
					switchesByRackPositionUid.TryGetValue(cable.SwitchRackPositionUID, out sw);
					resolvedSwitchById = (UnityEngine.Object)(object)sw != (UnityEngine.Object)null;
				}
				if ((UnityEngine.Object)(object)srv == (UnityEngine.Object)null || (UnityEngine.Object)(object)sw == (UnityEngine.Object)null) continue;
				if (srv.cablelinks == null || sw.cableLinkSwitchPorts == null) continue;
				if (cable.ServerPortIndex >= ((Il2CppArrayBase<CableLink>)(object)srv.cablelinks).Length) continue;
				if (cable.SwitchPortIndex >= ((Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts).Length) continue;
				CableLink srvPort = ((Il2CppArrayBase<CableLink>)(object)srv.cablelinks)[cable.ServerPortIndex];
				CableLink swPort = ((Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)[cable.SwitchPortIndex];
				if ((UnityEngine.Object)(object)srvPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)swPort == (UnityEngine.Object)null) continue;

				if (!string.IsNullOrEmpty(cable.PatchPanelID) && cable.PatchNearPortIndex >= 0 && cable.PatchFarPortIndex >= 0)
				{
					bool resolvedPanelById = !string.IsNullOrEmpty(cable.PatchPanelID) && panelsById.TryGetValue(cable.PatchPanelID, out panel);
					if (!resolvedPanelById && cable.PatchPanelRackPositionUID > 0)
					{
						panelsByRackPositionUid.TryGetValue(cable.PatchPanelRackPositionUID, out panel);
						resolvedPanelById = (UnityEngine.Object)(object)panel != (UnityEngine.Object)null;
					}
					if ((UnityEngine.Object)(object)panel == (UnityEngine.Object)null) continue;
					if (panel.cableLinkPorts == null) continue;
					if (cable.PatchNearPortIndex >= ((Il2CppArrayBase<CableLink>)(object)panel.cableLinkPorts).Length) continue;
					if (cable.PatchFarPortIndex >= ((Il2CppArrayBase<CableLink>)(object)panel.cableLinkPorts).Length) continue;
					CableLink patchNear = ((Il2CppArrayBase<CableLink>)(object)panel.cableLinkPorts)[cable.PatchNearPortIndex];
					CableLink patchFar = ((Il2CppArrayBase<CableLink>)(object)panel.cableLinkPorts)[cable.PatchFarPortIndex];
					if ((UnityEngine.Object)(object)patchNear == (UnityEngine.Object)null || (UnityEngine.Object)(object)patchFar == (UnityEngine.Object)null) continue;
					if (srvPort.cableIDsOnLink <= 0 && patchNear.cableIDsOnLink <= 0 && patchFar.cableIDsOnLink <= 0 && swPort.cableIDsOnLink <= 0)
					{
						Rack prevRack = _selectedRack;
						try
						{
							Rack routeRack = ResolveRackForRoute(srvPort, patchNear, patchFar, swPort);
							if ((UnityEngine.Object)(object)routeRack != (UnityEngine.Object)null)
								_selectedRack = routeRack;
							var serverPatchClips = FindCableClips(srvPort, patchNear);
							var switchPatchClips = FindCableClips(swPort, patchFar);
							bool ok1 = CreateCable(cp, srvPort, patchNear, serverPatchClips, CableLink.TypeOfLink.Server, CableLink.TypeOfLink.PatchPanel, srv.ServerID);
							bool ok2 = CreateCable(cp, swPort, patchFar, switchPatchClips, CableLink.TypeOfLink.Switch, CableLink.TypeOfLink.PatchPanel, srv.ServerID);
							if (ok1 && ok2)
							{
								restored += 2;
								if (!resolvedServerById || !resolvedSwitchById || !resolvedPanelById)
									restoredByRackPositionFallback += 2;
							}
						}
						finally
						{
							_selectedRack = prevRack;
						}
					}
					continue;
				}

				// Only recreate direct links if ports are not already wired.
				if (srvPort.cableIDsOnLink <= 0 && swPort.cableIDsOnLink <= 0)
				{
					Rack prevRack = _selectedRack;
					try
					{
						Rack routeRack = ResolveRackForRoute(srvPort, swPort);
						if ((UnityEngine.Object)(object)routeRack != (UnityEngine.Object)null)
							_selectedRack = routeRack;
						var waypoints = FindCableClips(srvPort, swPort);
						if (CreateCable(cp, srvPort, swPort, waypoints, CableLink.TypeOfLink.Server, CableLink.TypeOfLink.Switch, srv.ServerID))
						{
							restored++;
							if (!resolvedServerById || !resolvedSwitchById)
								restoredByRackPositionFallback++;
						}
					}
					finally
					{
						_selectedRack = prevRack;
					}
				}
			}
			if (restored > 0)
				((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Restored {restored} cables from RackCables.json" + ((restoredByRackPositionFallback > 0) ? $" ({restoredByRackPositionFallback} via rack-position fallback)" : ""));
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Warning($"Failed to restore cable topology: {ex.Message}");
		}
	}

	/// <summary>
	/// Zeros connectionSpeed on every empty SFP port (no inserted module) across all switches in the scene.
	/// <summary>
	/// Clears cableIDsOnLink on any CableLink whose stored cable ID has no corresponding entry in
	/// CablePositions. This happens after save/reload because the game serializes cableIDsOnLink
	/// (set by our mod) but not the cable routing data we added to CablePositions. Without this,
	/// auto-wire skips ports that look occupied but have no visual cable.
	/// </summary>
	private void ClearOrphanedCableIds()
	{
		CablePositions cp = UnityEngine.Object.FindObjectOfType<CablePositions>();
		if ((UnityEngine.Object)(object)cp == (UnityEngine.Object)null) return;
		int cleared = 0;
		foreach (CableLink cl in UnityEngine.Object.FindObjectsOfType<CableLink>())
		{
			if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null) continue;
			int id = cl.cableIDsOnLink;
			if (id <= 0) continue;
			// Check whether the cable actually has routing data in CablePositions.
			bool exists = false;
			try
			{
				var pts = cp.GetCablePositions(id);
				exists = pts != null && pts.Count >= 2;
			}
			catch { }
			if (!exists)
			{
				cl.cableIDsOnLink = -1;
				cleared++;
			}
		}
		if (cleared > 0)
			((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Cleared {cleared} orphaned cable IDs before re-wire.");
	}

	/// Prevents the prefab-baked default speed (often 50G) from being displayed on vacant SFP slots.
	/// </summary>
	private void ClearEmptySfpPortSpeeds()
	{
		int cleared = 0;
		foreach (NetworkSwitch sw in UnityEngine.Object.FindObjectsOfType<NetworkSwitch>())
		{
			if ((UnityEngine.Object)(object)sw == (UnityEngine.Object)null || sw.cableLinkSwitchPorts == null) continue;
			foreach (CableLink cl in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
			{
				if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null) continue;
				if (!cl.isSFPPort) continue;
				if ((UnityEngine.Object)(object)cl.insertedSFP != (UnityEngine.Object)null) continue; // occupied
				if (cl.cableIDsOnLink > 0) continue; // wired port — don't touch speed
				if (cl.connectionSpeed != 0f)
				{
					cl.connectionSpeed = 0f;
					cleared++;
				}
			}
		}
		if (cleared > 0)
			((MelonBase)this).LoggerInstance.Msg($"[RackBuilder] Cleared stale speed on {cleared} empty SFP ports.");
	}

	/// <summary>Runs the wiring logic for the current _selectedRack without logging to the UI.</summary>
	private void AutowireRackSilent(ref int wiredCount)
	{
		if (_disableAutoWireForDebug) return;
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null) return;
		CablePositions cp = UnityEngine.Object.FindObjectOfType<CablePositions>();
		if ((UnityEngine.Object)(object)cp == (UnityEngine.Object)null) return;
		AutoWireRack(); // reuses all the existing cable-creation logic; skips already-wired ports
	}

	private IEnumerator ScrubPlacedDeviceCableIds(GameObject go, int frames)
	{
		CablePositions cablePositions = UnityEngine.Object.FindObjectOfType<CablePositions>();
		for (int i = 0; i < frames; i++)
		{
			yield return null;
			if ((UnityEngine.Object)(object)go == (UnityEngine.Object)null)
				yield break;
			foreach (CableLink cl in go.GetComponentsInChildren<CableLink>(true))
			{
				if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null) continue;
				int id = cl.cableIDsOnLink;
				if (id > 0 && _autoWireProtectedCableIds.Contains(id)) continue; // AutoWire cable — keep it
				if (id > 0)
				{
					// Remove from the global registry so the game stops showing phantom traffic.
					try
					{
						if ((UnityEngine.Object)(object)cablePositions != (UnityEngine.Object)null)
							cablePositions.RemovePosition(id);
					}
					catch { }
				}
				cl.cableIDsOnLink = -1;
			}
			Server srv = go.GetComponent<Server>() ?? go.GetComponentInChildren<Server>();
			if ((UnityEngine.Object)(object)srv != (UnityEngine.Object)null)
			{
				if (srv.cablelinks != null)
				{
					foreach (CableLink serverLink in (Il2CppArrayBase<CableLink>)(object)srv.cablelinks)
					{
						if ((UnityEngine.Object)(object)serverLink == (UnityEngine.Object)null)
							continue;
						if (serverLink.cableIDsOnLink > 0 && _autoWireProtectedCableIds.Contains(serverLink.cableIDsOnLink)) continue;
						serverLink.cableIDsOnLink = -1;
					}
				}
				if (i == frames - 1)
				{
					try
					{
						bool stillConnected = srv.IsAnyCableConnected();
						int activeCount = (srv.activeLinks != null) ? srv.activeLinks.Count : -1;
						int nonZeroIds = 0;
						int nonNegativeIds = 0;
						int nonZeroSpeed = 0;
						int flaggedEndpoints = 0;
						int nonNoneTypes = 0;
						int customerTagged = 0;
						int insertedSfps = 0;
						if (srv.cablelinks != null)
						{
							foreach (CableLink serverLink2 in (Il2CppArrayBase<CableLink>)(object)srv.cablelinks)
							{
								if ((UnityEngine.Object)(object)serverLink2 == (UnityEngine.Object)null)
									continue;
								if (serverLink2.cableIDsOnLink > 0)
									nonZeroIds++;
								if (serverLink2.cableIDsOnLink >= 0)
									nonNegativeIds++;
								if (serverLink2.connectionSpeed > 0f)
									nonZeroSpeed++;
								if (serverLink2.isStartOrEnd || serverLink2.isEndPoint)
									flaggedEndpoints++;
								if (serverLink2.typeOfLink != CableLink.TypeOfLink.None)
									nonNoneTypes++;
								if (serverLink2.CustomerID >= 0)
									customerTagged++;
								if ((UnityEngine.Object)(object)serverLink2.insertedSFP != (UnityEngine.Object)null)
									insertedSfps++;
							}
						}
						((MelonBase)this).LoggerInstance.Msg($"[ScrubPlacedDeviceCableIds] Server connected after scrub: {stillConnected} | activeLinks={activeCount} | ids>0={nonZeroIds} | ids>=0={nonNegativeIds} | speed>0={nonZeroSpeed} | endpointFlags={flaggedEndpoints} | linkTypes={nonNoneTypes} | customerTags={customerTagged} | sfps={insertedSfps} | isOn={srv.isOn}");
					}
					catch (Exception ex)
					{
						((MelonBase)this).LoggerInstance.Warning("Server scrub status check failed: " + ex.Message);
					}
				}
			}
			NetworkSwitch sw = go.GetComponent<NetworkSwitch>() ?? go.GetComponentInChildren<NetworkSwitch>();
			if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null)
			{
				if (sw.cableLinkSwitchPorts != null)
				{
					foreach (CableLink switchLink in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
					{
						if ((UnityEngine.Object)(object)switchLink == (UnityEngine.Object)null)
							continue;
						if (switchLink.cableIDsOnLink > 0 && _autoWireProtectedCableIds.Contains(switchLink.cableIDsOnLink)) continue;
						switchLink.cableIDsOnLink = -1;
					}
				}
				if (i == frames - 1)
				{
					try
					{
						bool stillConnected2 = sw.IsAnyCableConnected();
						int tempDisconnected = (sw.temporarilyDisconnectedCables != null) ? sw.temporarilyDisconnectedCables.Count : -1;
						int nonZeroIds2 = 0;
						int nonNegativeIds2 = 0;
						int nonZeroSpeed2 = 0;
						int flaggedEndpoints2 = 0;
						int nonNoneTypes2 = 0;
						int customerTagged2 = 0;
						int insertedSfps2 = 0;
						if (sw.cableLinkSwitchPorts != null)
						{
							foreach (CableLink switchLink2 in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
							{
								if ((UnityEngine.Object)(object)switchLink2 == (UnityEngine.Object)null)
									continue;
								if (switchLink2.cableIDsOnLink > 0)
									nonZeroIds2++;
								if (switchLink2.cableIDsOnLink >= 0)
									nonNegativeIds2++;
								if (switchLink2.connectionSpeed > 0f)
									nonZeroSpeed2++;
								if (switchLink2.isStartOrEnd || switchLink2.isEndPoint)
									flaggedEndpoints2++;
								if (switchLink2.typeOfLink != CableLink.TypeOfLink.None)
									nonNoneTypes2++;
								if (switchLink2.CustomerID >= 0)
									customerTagged2++;
								if ((UnityEngine.Object)(object)switchLink2.insertedSFP != (UnityEngine.Object)null)
									insertedSfps2++;
							}
						}
						((MelonBase)this).LoggerInstance.Msg($"[ScrubPlacedDeviceCableIds] Switch connected after scrub: {stillConnected2} | tempDisconnected={tempDisconnected} | ids>0={nonZeroIds2} | ids>=0={nonNegativeIds2} | speed>0={nonZeroSpeed2} | endpointFlags={flaggedEndpoints2} | linkTypes={nonNoneTypes2} | customerTags={customerTagged2} | sfps={insertedSfps2} | isOn={sw.isOn}");
					}
					catch (Exception ex2)
					{
						((MelonBase)this).LoggerInstance.Warning("Switch scrub status check failed: " + ex2.Message);
					}
				}
			}
		}
	}

	private IEnumerator FinalizePlacedDeviceDeferred(GameObject go, RackPosition rp, int storedPosition, int sizeInU)
	{
		yield return null;
		yield return null;
		if ((UnityEngine.Object)(object)go == (UnityEngine.Object)null)
			yield break;
		UsableObject uo = go.GetComponent<UsableObject>() ?? go.GetComponentInChildren<UsableObject>();
		if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null)
			yield break;
		Server srv = go.GetComponent<Server>() ?? go.GetComponentInChildren<Server>();
		NetworkSwitch sw = go.GetComponent<NetworkSwitch>() ?? go.GetComponentInChildren<NetworkSwitch>();
		PatchPanel pp = go.GetComponent<PatchPanel>() ?? go.GetComponentInChildren<PatchPanel>();
		// Explicitly avoid game insert lifecycle hooks on placement so no implicit cable passes run.
		// Wiring is only performed when user clicks auto-wire buttons.
		// currentRackPosition is intentionally NOT set here ? the route evaluator uses it to
		// discover devices and auto-assigns cable IDs. We defer it until AutoWireRack().
		uo.storedPosition = storedPosition;
		uo.sizeInU = sizeInU;
		uo.objectInHands = false;
		uo.isDropAllowed = true;
		uo.keepUpright = false;
		if ((UnityEngine.Object)(object)srv != (UnityEngine.Object)null)
		{
			if (srv.activeLinks != null)
				srv.activeLinks.Clear();
			srv.currentProcessingSpeed = 0f;
			srv.previousProcessingSpeed = 0f;
		}
		foreach (CableLink cl in go.GetComponentsInChildren<CableLink>(true))
		{
			if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null) continue;
			cl.cableIDsOnLink = -1;
			cl.connectionSpeed = 0f;
		}
		MelonCoroutines.Start(EnableCollidersDelayed(go));
	}

	private void OpenAllWalls()
	{
		Il2CppArrayBase<Wall> val = UnityEngine.Object.FindObjectsOfType<Wall>();
		int num = 0;
		foreach (Wall item in val)
		{
			if (!((UnityEngine.Object)(object)item == (UnityEngine.Object)null) && !item.isWallOpened)
			{
				item.OpenWall();
				num++;
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Opened {num} walls");
		ShowRackList();
	}

	private void TryIntegrate()
	{
		_shop = UnityEngine.Object.FindObjectOfType<ComputerShop>();
		if ((UnityEngine.Object)(object)_shop == (UnityEngine.Object)null || (UnityEngine.Object)(object)_shop.mainScreen == (UnityEngine.Object)null)
		{
			return;
		}
		try
		{
			Transform val = _shop.mainScreen.transform.Find("Button Grid");
			if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
			{
				return;
			}
			Transform val2 = val.Find("Icon Hire bcg");
			if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
			{
				return;
			}
			GameObject val3 = UnityEngine.Object.Instantiate<GameObject>(((Component)val2).gameObject, val);
			((UnityEngine.Object)val3).name = "Icon Rack Manager bcg";
			Il2CppArrayBase<TextMeshProUGUI> componentsInChildren = val3.GetComponentsInChildren<TextMeshProUGUI>();
			foreach (TextMeshProUGUI item in componentsInChildren)
			{
				if ((UnityEngine.Object)(object)item != (UnityEngine.Object)null)
				{
					((TMP_Text)item).text = "Rack\nManager";
				}
			}
			ButtonExtended componentInChildren = val3.GetComponentInChildren<ButtonExtended>();
			if ((UnityEngine.Object)(object)componentInChildren != (UnityEngine.Object)null)
			{
				((UnityEventBase)componentInChildren.onClick).RemoveAllListeners();
				((UnityEvent)componentInChildren.onClick).AddListener((Action)OnRackManagerClicked);
			}
			GameObject balanceSheetScreen = _shop.balanceSheetScreen;
			if ((UnityEngine.Object)(object)balanceSheetScreen == (UnityEngine.Object)null)
			{
				return;
			}
			_rackScreen = UnityEngine.Object.Instantiate<GameObject>(balanceSheetScreen, balanceSheetScreen.transform.parent);
			((UnityEngine.Object)_rackScreen).name = "RackManager - OFF";
			_rackScreen.SetActive(false);
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < _rackScreen.transform.childCount; i++)
			{
				Transform child = _rackScreen.transform.GetChild(i);
				if (!((UnityEngine.Object)(object)child == (UnityEngine.Object)null) && !((UnityEngine.Object)child).name.Contains("Scroll View") && !((UnityEngine.Object)child).name.Contains("Button Return"))
				{
					list.Add(((Component)child).gameObject);
				}
			}
			foreach (GameObject item2 in list)
			{
				UnityEngine.Object.Destroy((UnityEngine.Object)(object)item2);
			}
			Il2CppArrayBase<ScrollRect> componentsInChildren2 = _rackScreen.GetComponentsInChildren<ScrollRect>(true);
			foreach (ScrollRect item3 in componentsInChildren2)
			{
				if ((UnityEngine.Object)(object)item3 == (UnityEngine.Object)null || (UnityEngine.Object)(object)item3.content == (UnityEngine.Object)null)
				{
					continue;
				}
				_contentParent = (Transform)(object)item3.content;
				List<Transform> list2 = new List<Transform>();
				for (int j = 0; j < _contentParent.childCount; j++)
				{
					list2.Add(_contentParent.GetChild(j));
				}
				foreach (Transform item4 in list2)
				{
					UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)item4).gameObject);
				}
				VerticalLayoutGroup val4 = ((Component)_contentParent).gameObject.GetComponent<VerticalLayoutGroup>();
				if ((UnityEngine.Object)(object)val4 == (UnityEngine.Object)null)
				{
					val4 = ((Component)_contentParent).gameObject.AddComponent<VerticalLayoutGroup>();
				}
				((HorizontalOrVerticalLayoutGroup)val4).spacing = 3f;
				((HorizontalOrVerticalLayoutGroup)val4).childForceExpandWidth = true;
				((HorizontalOrVerticalLayoutGroup)val4).childForceExpandHeight = false;
				((HorizontalOrVerticalLayoutGroup)val4).childControlWidth = true;
				((HorizontalOrVerticalLayoutGroup)val4).childControlHeight = false;
				ContentSizeFitter val5 = ((Component)_contentParent).gameObject.GetComponent<ContentSizeFitter>();
				if ((UnityEngine.Object)(object)val5 == (UnityEngine.Object)null)
				{
					val5 = ((Component)_contentParent).gameObject.AddComponent<ContentSizeFitter>();
				}
				val5.verticalFit = (ContentSizeFitter.FitMode)2;
				break;
			}
			Il2CppArrayBase<ButtonExtended> componentsInChildren3 = _rackScreen.GetComponentsInChildren<ButtonExtended>();
			foreach (ButtonExtended item5 in componentsInChildren3)
			{
				if ((UnityEngine.Object)(object)item5 != (UnityEngine.Object)null && ((UnityEngine.Object)((Component)item5).gameObject).name.Contains("Return"))
				{
					((UnityEventBase)item5.onClick).RemoveAllListeners();
					((UnityEvent)item5.onClick).AddListener((Action)OnReturnClicked);
					break;
				}
			}
			_integrated = true;
			((MelonBase)this).LoggerInstance.Msg("Rack Manager integrated into laptop!");
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Error("Integration failed: " + ex.Message);
			_integrated = true;
		}
	}

	private void OnRackManagerClicked()
	{
		// Lazy save-reload check: runs only when the player opens the UI, not every 2 seconds.
		CheckForSaveReload();
		_shop.mainScreen.SetActive(false);
		_rackScreen.SetActive(true);
		_onDetailPage = false;
		BuildItemChoices();
		ShowRackList();
	}

	private void OnReturnClicked()
	{
		if (_onDetailPage)
		{
			_onDetailPage = false;
			_selectedRack = null;
			_pendingBulkClearConfirmation = false;
			ShowRackList();
		}
		else
		{
			_rackScreen.SetActive(false);
			_shop.mainScreen.SetActive(true);
		}
	}

	private void ShowRackList()
	{
		if (_suppressUiUpdates) return;
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Expected O, but got Unknown
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Expected O, but got Unknown
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Expected O, but got Unknown
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Expected O, but got Unknown
		ClearContent();
		_pendingBulkClearConfirmation = false;
		_allRacks.Clear();
		Il2CppArrayBase<Rack> val = UnityEngine.Object.FindObjectsOfType<Rack>();
		foreach (Rack item in val)
		{
			if ((UnityEngine.Object)(object)item != (UnityEngine.Object)null)
			{
				_allRacks.Add(item);
			}
		}
		if ((UnityEngine.Object)(object)_pendingRemoveMount != (UnityEngine.Object)null)
		{
			ShowRemoveConfirmation();
			return;
		}
		AddTitle("Data Center Floor Plan");
		Il2CppArrayBase<Wall> val2 = UnityEngine.Object.FindObjectsOfType<Wall>();
		int num = 0;
		if (val2 != null)
		{
			foreach (Wall item2 in val2)
			{
				if ((UnityEngine.Object)(object)item2 != (UnityEngine.Object)null && !item2.isWallOpened)
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			AddClickableRow("  + MORE SPACE (free)", new Color(0.3f, 0.15f, 0.4f), delegate
			{
				OpenAllWalls();
			});
		}
		Il2CppArrayBase<RackMount> val3 = UnityEngine.Object.FindObjectsOfType<RackMount>();
		if (val3 == null || val3.Length == 0)
		{
			AddLabel("No mounts.");
			return;
		}
		List<RackMount> list = new List<RackMount>();
		foreach (RackMount item3 in val3)
		{
			if ((UnityEngine.Object)(object)item3 != (UnityEngine.Object)null)
			{
				list.Add(item3);
			}
		}
		SortedSet<int> sortedSet = new SortedSet<int>();
		foreach (RackMount item4 in list)
		{
			sortedSet.Add(Mathf.RoundToInt(((Component)item4).transform.position.x * 10f));
		}
		List<int> list2 = new List<int>(sortedSet);
		list2.Reverse();
		HashSet<int> hashSet = new HashSet<int>();
		for (int num2 = 1; num2 < list2.Count; num2++)
		{
			if (Mathf.Abs(list2[num2] - list2[num2 - 1]) > 12)
			{
				hashSet.Add(num2);
			}
		}
		List<int> list3 = new List<int>();
		int num3 = 0;
		for (int num4 = 0; num4 < list2.Count; num4++)
		{
			if (hashSet.Contains(num4))
			{
				list3.Add(num4 - num3);
				num3 = num4;
			}
		}
		list3.Add(list2.Count - num3);
		((MelonBase)this).LoggerInstance.Msg($"Grid pattern: {string.Join("-", list3)} ({list2.Count} columns)");
		SortedSet<int> sortedSet2 = new SortedSet<int>();
		foreach (RackMount item5 in list)
		{
			sortedSet2.Add(Mathf.RoundToInt(((Component)item5).transform.position.z * 10f));
		}
		List<int> list4 = new List<int>(sortedSet2);
		Dictionary<long, RackMount> dictionary = new Dictionary<long, RackMount>();
		foreach (RackMount item6 in list)
		{
			int num5 = Mathf.RoundToInt(((Component)item6).transform.position.x * 10f);
			int num6 = Mathf.RoundToInt(((Component)item6).transform.position.z * 10f);
			long key = ((long)num5 << 32) | (uint)num6;
			dictionary[key] = item6;
		}
		AddLabel($"  {_allRacks.Count} racks  |  Grey=empty  Green=installed  |  Click to manage");
		AddSpacer();
		foreach (int item7 in list4)
		{
			GameObject val4 = new GameObject("R");
			val4.transform.SetParent(_contentParent, false);
			val4.AddComponent<RectTransform>().sizeDelta = new Vector2(0f, 22f);
			LayoutElement val5 = val4.AddComponent<LayoutElement>();
			val5.preferredHeight = 22f;
			val5.minHeight = 22f;
			HorizontalLayoutGroup val6 = val4.AddComponent<HorizontalLayoutGroup>();
			((HorizontalOrVerticalLayoutGroup)val6).spacing = 1f;
			((HorizontalOrVerticalLayoutGroup)val6).childForceExpandWidth = true;
			((HorizontalOrVerticalLayoutGroup)val6).childForceExpandHeight = true;
			_uiRows.Add(val4);
			for (int num7 = 0; num7 < list2.Count; num7++)
			{
				if (hashSet.Contains(num7))
				{
					GameObject val7 = new GameObject("Gap");
					val7.transform.SetParent(val4.transform, false);
					val7.AddComponent<RectTransform>();
					LayoutElement val8 = val7.AddComponent<LayoutElement>();
					val8.preferredWidth = 8f;
					val8.minWidth = 8f;
					val8.flexibleWidth = 0f;
				}
				int num8 = list2[num7];
				long key2 = ((long)num8 << 32) | (uint)item7;
				GameObject val9 = new GameObject("C");
				val9.transform.SetParent(val4.transform, false);
				val9.AddComponent<RectTransform>();
				Image val10 = val9.AddComponent<Image>();
				Button val11 = val9.AddComponent<Button>();
				ColorBlock colors = ((Selectable)val11).colors;
				if (dictionary.TryGetValue(key2, out var value))
				{
					Rack componentInChildren = ((Component)value).GetComponentInChildren<Rack>();
					bool flag = (UnityEngine.Object)(object)componentInChildren != (UnityEngine.Object)null;
					if (!flag && value.isRackInstantiated)
					{
						value.isRackInstantiated = false;
					}
					if (flag)
					{
						((Graphic)val10).color = new Color(0.15f, 0.4f, 0.15f);
						colors.highlightedColor = new Color(0.25f, 0.55f, 0.25f);
						GameObject val12 = new GameObject("Util");
						val12.transform.SetParent(val9.transform, false);
						RectTransform val13 = val12.AddComponent<RectTransform>();
						val13.anchorMin = Vector2.zero;
						val13.anchorMax = Vector2.one;
						val13.sizeDelta = Vector2.zero;
						TextMeshProUGUI val14 = val12.AddComponent<TextMeshProUGUI>();
						((TMP_Text)val14).text = GetRackUtilizationText(componentInChildren);
						((TMP_Text)val14).fontSize = 8f;
						((TMP_Text)val14).alignment = (TextAlignmentOptions)514;
						((TMP_Text)val14).enableWordWrapping = false;
						((Graphic)val14).color = new Color(0.9f, 1f, 0.9f);
						((Graphic)val14).raycastTarget = false;
						Rack r = componentInChildren;
						RackMount m = value;
						((UnityEvent)val11.onClick).AddListener((Action)delegate
						{
							if ((UnityEngine.Object)(object)r != (UnityEngine.Object)null)
							{
								_selectedRack = r;
								_pendingBulkClearConfirmation = false;
								_onDetailPage = true;
								ShowRackDetail();
							}
						});
						EventTrigger val15 = val9.AddComponent<EventTrigger>();
						EventTrigger.Entry val16 = new EventTrigger.Entry();
						val16.eventID = (EventTriggerType)4;
						((UnityEvent<BaseEventData>)(object)val16.callback).AddListener((Action<BaseEventData>)delegate(BaseEventData data)
						{
							//IL_000c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0012: Invalid comparison between Unknown and I4
							PointerEventData val17 = ((Il2CppObjectBase)data).TryCast<PointerEventData>();
							if (val17 != null && (int)val17.button == 1)
							{
								_pendingRemoveMount = m;
								ShowRackList();
							}
						});
						val15.triggers.Add(val16);
					}
					else
					{
						((Graphic)val10).color = new Color(0.3f, 0.3f, 0.3f);
						colors.highlightedColor = new Color(0.5f, 0.5f, 0.5f);
						RackMount m2 = value;
						((UnityEvent)val11.onClick).AddListener((Action)delegate
						{
							InstallRackAtMount(m2);
						});
					}
				}
				else
				{
					((Graphic)val10).color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
					((Selectable)val11).interactable = false;
				}
				((Selectable)val11).colors = colors;
			}
		}
	}

	private string GetRackUtilizationText(Rack rack)
	{
		if ((UnityEngine.Object)(object)rack == (UnityEngine.Object)null || rack.positions == null)
			return "0/0U";
		int total = ((Il2CppArrayBase<RackPosition>)(object)rack.positions).Length;
		int used = 0;
		if (rack.isPositionUsed != null)
		{
			for (int i = 0; i < ((Il2CppArrayBase<int>)(object)rack.isPositionUsed).Length; i++)
			{
				if (((Il2CppArrayBase<int>)(object)rack.isPositionUsed)[i] != 0)
					used++;
			}
		}
		if (used < 0)
			used = 0;
		if (used > total)
			used = total;
		return $"{used}/{total}U";
	}

	private void ShowRemoveConfirmation()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		RackMount pendingRemoveMount = _pendingRemoveMount;
		if ((UnityEngine.Object)(object)pendingRemoveMount == (UnityEngine.Object)null)
		{
			ShowRackList();
			return;
		}
		Rack componentInChildren = ((Component)pendingRemoveMount).GetComponentInChildren<Rack>();
		int num = 0;
		if ((UnityEngine.Object)(object)componentInChildren != (UnityEngine.Object)null)
		{
			foreach (UsableObject componentsInChild in ((Component)componentInChildren).GetComponentsInChildren<UsableObject>())
			{
				if ((UnityEngine.Object)(object)componentsInChild != (UnityEngine.Object)null && (UnityEngine.Object)(object)((Component)componentsInChild).gameObject != (UnityEngine.Object)(object)((Component)componentInChildren).gameObject)
				{
					num++;
				}
			}
		}
		AddTitle("Remove Rack?");
		AddSpacer();
		AddColorLabel("  This will permanently remove the rack and", Color.white);
		AddColorLabel($"  ALL {num} items inside it.", new Color(1f, 0.7f, 0.3f));
		AddColorLabel("  All cables on those items will be disconnected.", new Color(1f, 0.7f, 0.3f));
		AddSpacer();
		AddClickableRow("  YES - Remove rack + contents", new Color(0.5f, 0.15f, 0.15f), delegate
		{
			RackMount pendingRemoveMount2 = _pendingRemoveMount;
			_pendingRemoveMount = null;
			RemoveRackAtMount(pendingRemoveMount2);
			ShowRackList();
		});
		AddSpacer();
		AddClickableRow("  NO - Cancel", new Color(0.2f, 0.2f, 0.2f), delegate
		{
			_pendingRemoveMount = null;
			ShowRackList();
		});
	}

	private void RemoveRackAtMount(RackMount mount)
	{
		if ((UnityEngine.Object)(object)mount == (UnityEngine.Object)null)
		{
			return;
		}
		Rack componentInChildren = ((Component)mount).GetComponentInChildren<Rack>();
		if ((UnityEngine.Object)(object)componentInChildren == (UnityEngine.Object)null)
		{
			mount.isRackInstantiated = false;
			return;
		}
		CablePositions val = UnityEngine.Object.FindObjectOfType<CablePositions>();
		int num = 0;
		foreach (CableLink componentsInChild in ((Component)componentInChildren).GetComponentsInChildren<CableLink>(true))
		{
			if ((UnityEngine.Object)(object)componentsInChild == (UnityEngine.Object)null)
			{
				continue;
			}
			int cableIDsOnLink = componentsInChild.cableIDsOnLink;
			if (cableIDsOnLink <= 0)
			{
				continue;
			}
			try
			{
				if ((UnityEngine.Object)(object)val != (UnityEngine.Object)null)
				{
					val.RemovePosition(cableIDsOnLink);
				}
			}
			catch
			{
			}
			componentsInChild.cableIDsOnLink = -1;
			num++;
		}
		int num2 = 0;
		foreach (UsableObject componentsInChild2 in ((Component)componentInChildren).GetComponentsInChildren<UsableObject>())
		{
			if (!((UnityEngine.Object)(object)componentsInChild2 == (UnityEngine.Object)null) && !((UnityEngine.Object)(object)((Component)componentsInChild2).gameObject == (UnityEngine.Object)(object)((Component)componentInChildren).gameObject))
			{
				UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)componentsInChild2).gameObject);
				num2++;
			}
		}
		if (componentInChildren.isPositionUsed != null)
		{
			for (int i = 0; i < ((Il2CppArrayBase<int>)(object)componentInChildren.isPositionUsed).Length; i++)
			{
				((Il2CppArrayBase<int>)(object)componentInChildren.isPositionUsed)[i] = 0;
			}
		}
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)(object)componentInChildren)
		{
			_selectedRack = null;
			_onDetailPage = false;
		}
		UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)componentInChildren).gameObject);
		mount.isRackInstantiated = false;
		((MelonBase)this).LoggerInstance.Msg($"Removed rack at {((UnityEngine.Object)((Component)mount).gameObject).name}: {num2} items, {num} cables");
	}

	// Clears ghost cable IDs from stale prefabs/saves.
	// Valid finished cables should have exactly 2 endpoint ports; anything else is treated as ghost/stale state.
	private void SanitizeGhostCableIDs()
	{
		var linksById = new Dictionary<int, List<CableLink>>();
		var endpointCount = new Dictionary<int, int>();
		var invalidEndpointIds = new HashSet<int>();
		foreach (CableLink cl in UnityEngine.Object.FindObjectsOfType<CableLink>())
		{
			if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null) continue;
			int id = cl.cableIDsOnLink;
			if (id <= 0) continue;
			if (!linksById.TryGetValue(id, out var list))
			{
				list = new List<CableLink>();
				linksById[id] = list;
			}
			list.Add(cl);
			bool isEndpoint = (UnityEngine.Object)(object)cl.parentServer != (UnityEngine.Object)null
				|| (UnityEngine.Object)(object)cl.parentSwitch != (UnityEngine.Object)null
				|| (UnityEngine.Object)(object)cl.parentPatchPanel != (UnityEngine.Object)null
				|| cl.CustomerID >= 0
				|| cl.isStartOrEnd
				|| cl.isEndPoint;
			if (isEndpoint)
			{
				endpointCount[id] = endpointCount.TryGetValue(id, out int ep) ? ep + 1 : 1;
				if (cl.isSFPPort && (UnityEngine.Object)(object)cl.insertedSFP == (UnityEngine.Object)null)
					invalidEndpointIds.Add(id);
			}
		}

		List<int> badIds = new List<int>();
		foreach (KeyValuePair<int, List<CableLink>> kv in linksById)
		{
			int id = kv.Key;
			int endpoints = endpointCount.TryGetValue(id, out int ep) ? ep : 0;
			if (endpoints != 2 || invalidEndpointIds.Contains(id))
				badIds.Add(id);
		}
		if (badIds.Count == 0)
			return;

		CablePositions cablePositions = UnityEngine.Object.FindObjectOfType<CablePositions>();
		int cleared = 0;
		foreach (int id in badIds)
		{
			try
			{
				if ((UnityEngine.Object)(object)cablePositions != (UnityEngine.Object)null)
					cablePositions.RemovePosition(id);
			}
			catch
			{
			}
			foreach (CableLink cl in linksById[id])
			{
				if ((UnityEngine.Object)(object)cl != (UnityEngine.Object)null && cl.cableIDsOnLink == id)
				{
					cl.cableIDsOnLink = -1;
					cl.connectionSpeed = 0f;
					cleared++;
				}
			}
		}
		if (cleared > 0)
			((MelonBase)this).LoggerInstance.Msg($"[SanitizeCables] Cleared {cleared} links across {badIds.Count} ghost cable IDs");
	}

	private List<UsableObject> CollectRackUsableObjects()
	{
		List<UsableObject> result = new List<UsableObject>();
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null || _selectedRack.positions == null)
			return result;

		HashSet<int> rpInstanceIds = new HashSet<int>();
		HashSet<int> rpUids = new HashSet<int>();
		foreach (RackPosition rp in (Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)
		{
			if ((UnityEngine.Object)(object)rp == (UnityEngine.Object)null) continue;
			rpInstanceIds.Add(((Component)rp).gameObject.GetInstanceID());
			if (rp.rackPosGlobalUID > 0)
				rpUids.Add(rp.rackPosGlobalUID);
		}

		HashSet<int> seen = new HashSet<int>();
		foreach (UsableObject uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
		{
			if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null) continue;
			bool inRack = false;
			if ((UnityEngine.Object)(object)uo.currentRackPosition != (UnityEngine.Object)null)
			{
				int rpId = ((Component)uo.currentRackPosition).gameObject.GetInstanceID();
				inRack = rpInstanceIds.Contains(rpId);
			}
			if (!inRack && uo.rackPositionUID > 0)
				inRack = rpUids.Contains(uo.rackPositionUID);
			if (!inRack) continue;
			int id = ((UnityEngine.Object)(object)uo).GetInstanceID();
			if (seen.Add(id))
				result.Add(uo);
		}
		return result;
	}

	private List<Server> CollectRackServers()
	{
		return ExtractRackServers(CollectRackUsableObjects());
	}

	private List<NetworkSwitch> CollectRackSwitches()
	{
		return ExtractRackSwitches(CollectRackUsableObjects());
	}

	private List<Server> ExtractRackServers(IEnumerable<UsableObject> usableObjects)
	{
		List<Server> result = new List<Server>();
		foreach (UsableObject uo in usableObjects)
		{
			Server srv = ((Component)uo).GetComponent<Server>() ?? ((Component)uo).GetComponentInChildren<Server>();
			if ((UnityEngine.Object)(object)srv != (UnityEngine.Object)null)
				result.Add(srv);
		}
		return result;
	}

	private List<NetworkSwitch> ExtractRackSwitches(IEnumerable<UsableObject> usableObjects)
	{
		List<NetworkSwitch> result = new List<NetworkSwitch>();
		foreach (UsableObject uo in usableObjects)
		{
			NetworkSwitch sw = ((Component)uo).GetComponent<NetworkSwitch>() ?? ((Component)uo).GetComponentInChildren<NetworkSwitch>();
			if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null)
				result.Add(sw);
		}
		return result;
	}

	private List<PatchPanel> ExtractRackPatchPanels(IEnumerable<UsableObject> usableObjects)
	{
		List<PatchPanel> result = new List<PatchPanel>();
		foreach (UsableObject uo in usableObjects)
		{
			PatchPanel pp = ((Component)uo).GetComponent<PatchPanel>() ?? ((Component)uo).GetComponentInChildren<PatchPanel>();
			if ((UnityEngine.Object)(object)pp != (UnityEngine.Object)null)
				result.Add(pp);
		}
		return result;
	}

	private bool CreateCable(CablePositions cablePositions, CableLink startPort, CableLink endPort, IEnumerable<Transform> waypoints, CableLink.TypeOfLink startType, CableLink.TypeOfLink endType, string serverId = "")
	{
		if ((UnityEngine.Object)(object)cablePositions == (UnityEngine.Object)null || (UnityEngine.Object)(object)startPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)endPort == (UnityEngine.Object)null)
			return false;
		if (!IsPortReadyForCable(startPort) || !IsPortReadyForCable(endPort))
			return false;
		int cableId = 0;
		try
		{
			cableId = cablePositions.CreateNewCable();
			if (cableId <= 0)
				return false;
			cablePositions.AssignNewPosition(cableId, ((Component)startPort).transform, true, false, startType, serverId);
			foreach (Transform waypoint in waypoints)
			{
				if ((UnityEngine.Object)(object)waypoint != (UnityEngine.Object)null)
					cablePositions.AssignNewPosition(cableId, waypoint, false, false, CableLink.TypeOfLink.None, serverId);
			}
			cablePositions.AssignNewPosition(cableId, ((Component)endPort).transform, false, true, endType, serverId);
			cablePositions.GenerateFinalPath(cableId);
			cablePositions.RedrawCable(cableId);
			var cablePoints = cablePositions.GetCablePositions(cableId);
			if (cablePoints == null || cablePoints.Count < 2)
			{
				try
				{
					cablePositions.RemovePosition(cableId);
				}
				catch
				{
				}
				return false;
			}
			startPort.cableIDsOnLink = cableId;
			endPort.cableIDsOnLink = cableId;
			_autoWireProtectedCableIds.Add(cableId);
			startPort.isStartOrEnd = true;
			startPort.isEndPoint = true;
			endPort.isStartOrEnd = true;
			endPort.isEndPoint = true;
			startPort.typeOfLink = startType;
			endPort.typeOfLink = endType;
			float aSpeed = ((startPort.isSFPPort && (UnityEngine.Object)(object)startPort.insertedSFP != (UnityEngine.Object)null) ? startPort.insertedSFP.speed : startPort.connectionSpeed);
			float bSpeed = ((endPort.isSFPPort && (UnityEngine.Object)(object)endPort.insertedSFP != (UnityEngine.Object)null) ? endPort.insertedSFP.speed : endPort.connectionSpeed);
			float cableSpeed = 0f;
			if (aSpeed > 0f && bSpeed > 0f)
				cableSpeed = Math.Min(aSpeed, bSpeed);
			else if (aSpeed > 0f)
				cableSpeed = aSpeed;
			else if (bSpeed > 0f)
				cableSpeed = bSpeed;
			if (cableSpeed > 0f)
			{
				startPort.connectionSpeed = cableSpeed;
				endPort.connectionSpeed = cableSpeed;
			}
			return true;
		}
		catch
		{
			if (cableId > 0)
			{
				try
				{
					cablePositions.RemovePosition(cableId);
				}
				catch
				{
				}
			}
			return false;
		}
	}

	private static bool IsPortReadyForCable(CableLink port)
	{
		if ((UnityEngine.Object)(object)port == (UnityEngine.Object)null)
			return false;
		if (port.cableIDsOnLink > 0)
			return false;
		if (port.isSFPPort && (UnityEngine.Object)(object)port.insertedSFP == (UnityEngine.Object)null)
			return false;
		return true;
	}

	private static int GetServerPortIndex(CableLink serverPort)
	{
		if ((UnityEngine.Object)(object)serverPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)serverPort.parentServer == (UnityEngine.Object)null || serverPort.parentServer.cablelinks == null)
			return -1;
		int idx = 0;
		foreach (CableLink port in (Il2CppArrayBase<CableLink>)(object)serverPort.parentServer.cablelinks)
		{
			if ((UnityEngine.Object)(object)port == (UnityEngine.Object)null)
			{
				idx++;
				continue;
			}
			if (((UnityEngine.Object)(object)port).GetInstanceID() == ((UnityEngine.Object)(object)serverPort).GetInstanceID())
				return idx;
			idx++;
		}
		return -1;
	}

	private static int GetSwitchPortIndex(CableLink switchPort)
	{
		if ((UnityEngine.Object)(object)switchPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)switchPort.parentSwitch == (UnityEngine.Object)null || switchPort.parentSwitch.cableLinkSwitchPorts == null)
			return -1;
		int idx = 0;
		foreach (CableLink port in (Il2CppArrayBase<CableLink>)(object)switchPort.parentSwitch.cableLinkSwitchPorts)
		{
			if ((UnityEngine.Object)(object)port == (UnityEngine.Object)null)
			{
				idx++;
				continue;
			}
			if (((UnityEngine.Object)(object)port).GetInstanceID() == ((UnityEngine.Object)(object)switchPort).GetInstanceID())
				return idx;
			idx++;
		}
		return -1;
	}

	private static int GetPatchPanelPortIndex(CableLink patchPort)
	{
		if ((UnityEngine.Object)(object)patchPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)patchPort.parentPatchPanel == (UnityEngine.Object)null || patchPort.parentPatchPanel.cableLinkPorts == null)
			return -1;
		int idx = 0;
		foreach (CableLink port in (Il2CppArrayBase<CableLink>)(object)patchPort.parentPatchPanel.cableLinkPorts)
		{
			if ((UnityEngine.Object)(object)port == (UnityEngine.Object)null)
			{
				idx++;
				continue;
			}
			if (((UnityEngine.Object)(object)port).GetInstanceID() == ((UnityEngine.Object)(object)patchPort).GetInstanceID())
				return idx;
			idx++;
		}
		return -1;
	}

	private static int GetSwitchInstanceId(CableLink port)
	{
		if ((UnityEngine.Object)(object)port == (UnityEngine.Object)null || (UnityEngine.Object)(object)port.parentSwitch == (UnityEngine.Object)null)
			return -1;
		return ((UnityEngine.Object)(object)port.parentSwitch).GetInstanceID();
	}

	private static CableLink PickBestSwitchPort(CableLink targetPort, List<CableLink> candidatePorts, Dictionary<int, int> switchUsage, Func<CableLink, bool> compatibility)
	{
		if ((UnityEngine.Object)(object)targetPort == (UnityEngine.Object)null || candidatePorts == null)
			return null;
		CableLink best = null;
		float bestScore = float.MaxValue;
		foreach (CableLink candidate in candidatePorts)
		{
			if (!IsPortReadyForCable(candidate))
				continue;
			if (compatibility != null && !compatibility(candidate))
				continue;
			int swId = GetSwitchInstanceId(candidate);
			int used = (swId > 0 && switchUsage.TryGetValue(swId, out int val)) ? val : 0;
			float dist = (((Component)candidate).transform.position - ((Component)targetPort).transform.position).sqrMagnitude;
			float score = dist + used * 25f;
			if (score < bestScore)
			{
				bestScore = score;
				best = candidate;
			}
		}
		return best;
	}

	private void ShowRackDetail()
	{
		if (_suppressUiUpdates) return;
		//IL_139a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_1178: Unknown result type (might be due to invalid IL or missing references)
		ClearContent();
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null)
		{
			return;
		}
		// Intentionally do not run global ghost-cable sanitation here.
		// It can touch live non-rack links (for example customer cables).
		if (_itemChoices.Count == 0)
		{
			BuildItemChoices();
		}
		int num = ((_selectedRack.positions != null) ? ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length : 0);
		Il2CppStructArray<int> isPositionUsed = _selectedRack.isPositionUsed;
		int num2 = 0;
		if (isPositionUsed != null)
		{
			for (int i = 0; i < ((Il2CppArrayBase<int>)(object)isPositionUsed).Length; i++)
			{
				if (((Il2CppArrayBase<int>)(object)isPositionUsed)[i] != 0)
				{
					num2++;
				}
			}
		}
		int num3 = 0;
		int num4 = 0;
		foreach (KeyValuePair<int, int> item3 in _cartQty)
		{
			if (item3.Key >= 0 && item3.Key < _itemChoices.Count && item3.Value > 0)
			{
				num3 += _itemChoices[item3.Key].sizeInU * item3.Value;
				num4 += item3.Value;
			}
		}
		int num5 = num - num2 - num3;
		AddTitle("Rack Configuration");
		AddSpacer();
		AddColorLabel($"  Capacity: {num2}U used  +  {num3}U in cart  =  {num5}U free  /  {num}U total", (Color)((num5 >= 0) ? Color.white : new Color(1f, 0.3f, 0.3f)));
		AddSpacer();
		// Scan installed equipment: per-RackPosition (Pass 1) + broad fallback (Pass 2).
		List<(int, int, string, string, UsableObject)> list = new List<(int, int, string, string, UsableObject)>();
		HashSet<int> slotsFound = new HashSet<int>();

		// Build rackPositionUID ? slot-index lookup for Pass 2 (save-reload reliable mapping)
		Dictionary<int, int> rpUidToSlot = new Dictionary<int, int>();
		// Build RackPosition object-reference ? slot-index lookup for Pass 3 (collision-safe)
		Dictionary<int, int> rpInstanceToSlot = new Dictionary<int, int>();
		if (_selectedRack.positions != null)
		{
			for (int s = 0; s < num; s++)
			{
				RackPosition rp0 = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[s];
				// Skip UID=0 ? unassigned positions must never match unplaced objects (rackPositionUID default is also 0)
				if ((UnityEngine.Object)(object)rp0 != (UnityEngine.Object)null && rp0.rackPosGlobalUID != 0)
					rpUidToSlot[rp0.rackPosGlobalUID] = s;
				if ((UnityEngine.Object)(object)rp0 != (UnityEngine.Object)null)
					rpInstanceToSlot[((Component)rp0).gameObject.GetInstanceID()] = s;
			}
		}

		// Pass 1: walk each RackPosition's direct children for equipment
		if (_selectedRack.positions != null)
		{
			for (int j = 0; j < num; j++)
			{
				if (slotsFound.Contains(j)) continue;
				RackPosition rp = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[j];
				if ((UnityEngine.Object)(object)rp == (UnityEngine.Object)null) continue;
				Transform rpT = ((Component)rp).transform;
				for (int k = 0; k < rpT.childCount; k++)
				{
					Transform child = rpT.GetChild(k);
					if ((UnityEngine.Object)(object)child == (UnityEngine.Object)null) continue;
					string label = null;
					string colorType = null;
					int size = 1;
					UsableObject uo = null;
					// Try UsableObject (polymorphic ? covers Server/Switch/PatchPanel subclasses)
					uo = ((Component)child).GetComponent<UsableObject>();
					if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null)
						uo = ((Component)child).GetComponentInChildren<UsableObject>();
					if ((UnityEngine.Object)(object)uo != (UnityEngine.Object)null)
					{
						size = (uo.sizeInU > 0) ? uo.sizeInU : 1;
						label = DescribeUsableObject(uo);
						if (label != null) colorType = ClassifyLabel(label);
					}
					else
					{
						// IL2CPP polymorphism fallback: check concrete types directly
						Server srv = ((Component)child).GetComponentInChildren<Server>();
						if ((UnityEngine.Object)(object)srv != (UnityEngine.Object)null)
						{
							uo = (UsableObject)(object)srv;
							size = (((UsableObject)srv).sizeInU > 0) ? ((UsableObject)srv).sizeInU : 3;
							label = srv.isBroken ? $"Server {size}U BROKEN" : (srv.isOn ? $"Server {size}U [ON]" : $"Server {size}U [OFF]");
							colorType = "Server";
						}
						else
						{
							NetworkSwitch sw = ((Component)child).GetComponentInChildren<NetworkSwitch>();
							if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null)
							{
								uo = (UsableObject)(object)sw;
								size = 1;
								label = sw.isBroken ? "Switch BROKEN" : (sw.isOn ? "Switch [ON]" : "Switch [OFF]");
								colorType = "Switch";
							}
							else
							{
								PatchPanel pp = ((Component)child).GetComponentInChildren<PatchPanel>();
								if ((UnityEngine.Object)(object)pp != (UnityEngine.Object)null)
								{
									size = 1;
									label = "Patch Panel";
									colorType = "PatchPanel";
								}
							}
						}
					}
					if (label != null)
					{
						list.Add((j, size, label, colorType, uo));
						for (int u = j; u < j + size && u < num; u++) slotsFound.Add(u);
						break;
					}
				}
			}
		}

		// Pass 2: broad scan under Rack (and RackMount parent) for items not caught in Pass 1
		{
			Component searchRoot = (Component)(object)_selectedRack;
			RackMount parentMount = ((Component)_selectedRack).GetComponentInParent<RackMount>();
			if ((UnityEngine.Object)(object)parentMount != (UnityEngine.Object)null)
				searchRoot = (Component)(object)parentMount;
			Il2CppArrayBase<UsableObject> broadUos = searchRoot.GetComponentsInChildren<UsableObject>();
			if (broadUos != null)
			{
				foreach (UsableObject uo in (Il2CppArrayBase<UsableObject>)(object)broadUos)
				{
					if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null) continue;
					if (uo.rackPositionUID == 0) continue; // unplaced object ? skip
					int slot = -1;
					if (rpUidToSlot.TryGetValue(uo.rackPositionUID, out int mapped1))
						slot = mapped1;
					else
					{
						slot = uo.storedPosition;
						if (slot < 0 || slot >= num) continue;
					}
					if (slotsFound.Contains(slot)) continue;
					string label2 = DescribeUsableObject(uo);
					if (label2 == null) continue;
					string colorType2 = ClassifyLabel(label2);
					int size2 = (uo.sizeInU > 0) ? uo.sizeInU : 1;
					list.Add((slot, size2, label2, colorType2, uo));
					for (int u = slot; u < slot + size2 && u < num; u++) slotsFound.Add(u);
				}
			}
		}

		// Pass 3: catches items the game reparented to parentUsableObjects (Start() on UsableObject/Server).
		// Uses currentRackPosition object-reference ? immune to UID integer collisions across racks.
		if (slotsFound.Count < num)
		{
			foreach (UsableObject uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
			{
				if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null) continue;
				if ((UnityEngine.Object)(object)uo.currentRackPosition == (UnityEngine.Object)null) continue;
				int rpId = ((Component)uo.currentRackPosition).gameObject.GetInstanceID();
				if (!rpInstanceToSlot.TryGetValue(rpId, out int slot)) continue;
				if (slotsFound.Contains(slot)) continue;
				string label2 = DescribeUsableObject(uo);
				if (label2 == null) continue;
				string colorType2 = ClassifyLabel(label2);
				int size2 = (uo.sizeInU > 0) ? uo.sizeInU : 1;
				list.Add((slot, size2, label2, colorType2, uo));
				for (int u = slot; u < slot + size2 && u < num; u++) slotsFound.Add(u);
			}
		}

		if (_enableVerboseDiagnostics)
		{
			((MelonBase)this).LoggerInstance.Msg($"[ShowRackDetail] Pass1+2+3+4 total={list.Count}");
		}

		num5 = num - num2 - num3;
		if (list.Count > 0)
		{
			AddColorLabel("  Currently Installed:  (click to remove)", new Color(0.6f, 0.6f, 0.6f));
			list.Sort(((int anchorIdx, int size, string name, string colorType, UsableObject uo) a, (int anchorIdx, int size, string name, string colorType, UsableObject uo) b) => b.anchorIdx.CompareTo(a.anchorIdx));
			foreach (var item4 in list)
			{
				var (anchor, size, _, _, _) = item4;
				AddClickableRow($"    U{anchor + 1:D2}  |  {item4.Item3}  ({size}U)   [X]", GetColor(item4.Item4), delegate
				{
					RemoveItemByAnchor(anchor, size);
					ShowRackDetail();
				});
			}
			AddClickableRow("  BULK REMOVE - Clear entire rack", new Color(0.45f, 0.15f, 0.15f), delegate
			{
				_pendingBulkClearConfirmation = true;
				ShowRackDetail();
			});
			if (_pendingBulkClearConfirmation)
			{
				AddColorLabel("  Confirm remove ALL installed equipment from this rack?", new Color(1f, 0.7f, 0.3f));
				AddClickableRow("  YES - Remove all equipment", new Color(0.55f, 0.15f, 0.15f), delegate
				{
					int removed = RemoveAllItemsFromSelectedRack();
					_pendingBulkClearConfirmation = false;
					((MelonBase)this).LoggerInstance.Msg($"Bulk remove complete: removed {removed} installed items");
					ShowRackDetail();
					MelonCoroutines.Start(RefreshRackDetailDeferred(2));
				});
				AddClickableRow("  NO - Cancel bulk remove", new Color(0.2f, 0.2f, 0.2f), delegate
				{
					_pendingBulkClearConfirmation = false;
					ShowRackDetail();
				});
			}
			AddSpacer();
		}
		else
		{
			_pendingBulkClearConfirmation = false;
		}
		AddDivider();
		AddColorLabel("  Networking", new Color(1f, 0.8f, 0.3f));
		Color bgColor = default(Color);
		for (int num6 = 0; num6 < _itemChoices.Count; num6++)
		{
			ItemChoice itemChoice = _itemChoices[num6];
			if (!(itemChoice.category != "switch") || !(itemChoice.category != "patchpanel"))
			{
				bgColor = new Color(0.25f, 0.2f, 0.1f);
				AddQuantityRow(itemChoice.name, itemChoice.sizeInU, bgColor, num6, num5);
			}
		}
		AddSpacer();
		AddColorLabel("  Servers", new Color(0.4f, 0.7f, 1f));
		Color bgColor2 = default(Color);
		for (int num7 = 0; num7 < _itemChoices.Count; num7++)
		{
			ItemChoice itemChoice2 = _itemChoices[num7];
			if (!(itemChoice2.category != "server"))
			{
				bgColor2 = new Color(0.15f, 0.2f, 0.35f);
				AddQuantityRow(itemChoice2.name, itemChoice2.sizeInU, bgColor2, num7, num5);
			}
		}
		AddDivider();
		if (num4 > 0)
		{
			AddClickableRow($"  CONFIRM - Install {num4} items ({num3}U)", new Color(0.15f, 0.4f, 0.15f), delegate
			{
				InstallCartItems();
			});
			AddSpacer();
			AddClickableRow("  Clear Cart", new Color(0.4f, 0.1f, 0.1f), delegate
			{
				_cartQty.Clear();
				ShowRackDetail();
			});
		}
		AddDivider();
		int num8 = 0;
		int num9 = 0;
		List<UsableObject> rackUsableObjects = CollectRackUsableObjects();
		List<Server> rackServers = ExtractRackServers(rackUsableObjects);
		List<NetworkSwitch> rackSwitches = ExtractRackSwitches(rackUsableObjects);
		foreach (Server srv in rackServers)
		{
			bool wired = false;
			if (srv.cablelinks != null)
			{
				foreach (CableLink cl in (Il2CppArrayBase<CableLink>)(object)srv.cablelinks)
				{
					if ((UnityEngine.Object)(object)cl != (UnityEngine.Object)null && cl.cableIDsOnLink > 0)
					{
						wired = true;
						break;
					}
				}
			}
			if (!wired)
				num8++;
		}
		foreach (NetworkSwitch sw in rackSwitches)
		{
			if (sw.cableLinkSwitchPorts == null) continue;
			foreach (CableLink cl in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
			{
				if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null) continue;
				if (cl.cableIDsOnLink <= 0 && (!cl.isSFPPort || (UnityEngine.Object)(object)cl.insertedSFP != (UnityEngine.Object)null))
					num9++;
			}
		}
		if (num8 > 0 && num9 > 0)
		{
			AddClickableRow($"  AUTO-WIRE - {num8} unwired servers | {num9} free switch ports", new Color(0.1f, 0.35f, 0.1f), delegate
			{
				AutoWireRack();
			});
		}
		else if (num8 > 0)
		{
			AddColorLabel($"  {num8} unwired servers but no free switch ports", new Color(0.5f, 0.3f, 0.3f));
		}
		int num13 = 0;
		foreach (NetworkSwitch sw2 in rackSwitches)
		{
			if (sw2.cableLinkSwitchPorts == null) continue;
			foreach (CableLink cl2 in (Il2CppArrayBase<CableLink>)(object)sw2.cableLinkSwitchPorts)
			{
				if ((UnityEngine.Object)(object)cl2 != (UnityEngine.Object)null && cl2.isSFPPort && (UnityEngine.Object)(object)cl2.insertedSFP == (UnityEngine.Object)null)
					num13++;
			}
		}
		if (num13 > 0)
		{
			AddColorLabel($"  Auto-SFP: {num13} empty SFP/QSFP+ ports", Color.white);
			AddClickableRow("  AUTO-FILL - Insert modules into all empty SFP ports", new Color(0.1f, 0.3f, 0.4f), delegate
			{
				AutoFillSfpModules();
			});
		}
		string selectedRackRole = GetRackRole(_selectedRack);
		AddSpacer();
		AddColorLabel($"  Rack Role: {selectedRackRole.ToUpperInvariant()}", Color.white);
		AddClickableRow($"  TOGGLE ROLE - Set as {(selectedRackRole == RackRoleNetwork ? "SERVER" : "NETWORK")} rack", new Color(0.2f, 0.25f, 0.4f), delegate
		{
			ToggleRackRole(_selectedRack);
			ShowRackDetail();
		});
		int num15 = 0;
		foreach (NetworkSwitch sw3 in rackSwitches)
		{
			if (sw3.cableLinkSwitchPorts == null) continue;
			bool hasFreeUplinkPort = false;
			foreach (CableLink cl3 in (Il2CppArrayBase<CableLink>)(object)sw3.cableLinkSwitchPorts)
			{
				if ((UnityEngine.Object)(object)cl3 == (UnityEngine.Object)null) continue;
				if (cl3.cableIDsOnLink <= 0 && (!cl3.isSFPPort || (UnityEngine.Object)(object)cl3.insertedSFP != (UnityEngine.Object)null))
					hasFreeUplinkPort = true;
			}
			if (hasFreeUplinkPort)
				num15++;
		}
		int num17 = 0;
		Il2CppArrayBase<CustomerBase> val6 = UnityEngine.Object.FindObjectsOfType<CustomerBase>();

		foreach (CustomerBase item13 in val6)
		{
			if ((UnityEngine.Object)(object)item13 == (UnityEngine.Object)null)
				continue;
			if (item13.cableLinks == null) continue;
			foreach (CableLink item14 in (Il2CppArrayBase<CableLink>)(object)item13.cableLinks)
			{
				if ((UnityEngine.Object)(object)item14 != (UnityEngine.Object)null && item14.cableIDsOnLink <= 0)
					num17++;
			}
		}
		if (num15 <= 0)
		{
			return;
		}
		AddSpacer();
		if (selectedRackRole == RackRoleNetwork)
			AddColorLabel($"  Connect Network Rack to Customer: ({num15} switches need uplink)", Color.white);
		else
			AddColorLabel($"  Connect Server Rack via Network Rack to Customer: ({num15} source switches)", Color.white);
		bool flag4 = false;
		foreach (CustomerBase item15 in val6)
		{
			if ((UnityEngine.Object)(object)item15 == (UnityEngine.Object)null || item15.cableLinks == null || item15.customerID < 0)
			{
				continue;
			}
			int num21 = 0;
			foreach (CableLink item16 in (Il2CppArrayBase<CableLink>)(object)item15.cableLinks)
			{
				if ((UnityEngine.Object)(object)item16 != (UnityEngine.Object)null && item16.cableIDsOnLink <= 0)
				{
					num21++;
				}
			}
			if (num21 == 0)
			{
				continue;
			}
			int baseId = item15.customerBaseID;
			int customerID = item15.customerID;
			string value3 = $"Customer {customerID}";
			try
			{
				MainGameManager val8 = UnityEngine.Object.FindObjectOfType<MainGameManager>();
				if ((UnityEngine.Object)(object)val8 != (UnityEngine.Object)null)
				{
					CustomerItem customerItemByID = val8.GetCustomerItemByID(customerID);
					if ((UnityEngine.Object)(object)customerItemByID != (UnityEngine.Object)null && !string.IsNullOrEmpty(customerItemByID.customerName))
					{
						value3 = customerItemByID.customerName;
					}
				}
			}
			catch
			{
			}
			if (selectedRackRole == RackRoleNetwork)
			{
				AddClickableRow($"    {value3} ({num21} free ports)", new Color(0.1f, 0.3f, 0.4f), delegate
				{
					AutoWireToCustomer(baseId);
				});
			}
			else
			{
				AddClickableRow($"    {value3} via nearest NETWORK rack ({num21} free ports)", new Color(0.1f, 0.35f, 0.3f), delegate
				{
					AutoWireServerRackToCustomerViaNetwork(baseId);
				});
			}
			flag4 = true;
		}
		if (!flag4)
		{
			AddColorLabel("    No active customers with free ports", new Color(0.5f, 0.3f, 0.3f));
		}
	}

	private List<(int sfpType, float speed, int prefabIdx, string name)> BuildSfpPrefabList(MainGameManager mgr)
	{
		if (_sfpPrefabInfo != null)
		{
			return _sfpPrefabInfo;
		}
		List<(int, float, int, string)> list = new List<(int, float, int, string)>();
		if ((UnityEngine.Object)(object)mgr == (UnityEngine.Object)null || mgr.sfpPrefabs == null)
		{
			_sfpPrefabInfo = list;
			return list;
		}
		int length = ((Il2CppArrayBase<GameObject>)(object)mgr.sfpPrefabs).Length;
		for (int i = 0; i < length; i++)
		{
			GameObject val = ((Il2CppArrayBase<GameObject>)(object)mgr.sfpPrefabs)[i];
			if (!((UnityEngine.Object)(object)val == (UnityEngine.Object)null))
			{
				SFPModule val2 = val.GetComponent<SFPModule>() ?? val.GetComponentInChildren<SFPModule>();
				if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
				{
					((MelonBase)this).LoggerInstance.Msg($"  sfpPrefabs[{i}] {((UnityEngine.Object)val).name} ? no SFPModule component");
				}
				else
				{
					((MelonBase)this).LoggerInstance.Msg($"  sfpPrefabs[{i}] {((UnityEngine.Object)val).name} ? sfpType={val2.sfpType} speed={val2.speed}");
					list.Add((val2.sfpType, val2.speed, i, ((UnityEngine.Object)val).name));
				}
			}
		}
		_sfpPrefabInfo = list;
		return list;
	}

	private int FindBestSfpPrefab(List<(int sfpType, float speed, int prefabIdx, string name)> list, int requiredType, float requiredSpeed)
	{
		int num = -1;
		float num2 = -1f;
		int num3 = -1;
		float num4 = float.MaxValue;
		for (int i = 0; i < list.Count; i++)
		{
			(int, float, int, string) tuple = list[i];
			if (tuple.Item1 != requiredType)
				continue;
			float speed = tuple.Item2;
			if (requiredSpeed > 0f)
			{
				if (speed <= requiredSpeed + 0.01f && speed > num2)
				{
					num2 = speed;
					num = tuple.Item3;
				}
				float over = speed - requiredSpeed;
				if (over > 0f && over < num4)
				{
					num4 = over;
					num3 = tuple.Item3;
				}
			}
			else if (speed > num2)
			{
				num2 = speed;
				num = tuple.Item3;
			}
		}
		if (num >= 0)
			return num;
		if (num3 >= 0)
			return num3;
		if (list.Count == 0)
		{
			return -1;
		}
		string text = ((requiredSpeed >= 35f) ? "qsfp" : ((!(requiredSpeed >= 20f)) ? "sfp+" : "sfp28"));
		int num5 = -1;
		int num6 = -1;
		int num7 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			(int, float, int, string) tuple2 = list[j];
			string text2 = (tuple2.Item4 ?? "").ToLowerInvariant();
			if (text2.Contains("qsfp"))
			{
				if (num5 < 0)
				{
					num5 = tuple2.Item3;
				}
			}
			else if (text2.Contains("sfp28") || text2.Contains("sfp_28"))
			{
				if (num6 < 0)
				{
					num6 = tuple2.Item3;
				}
			}
			else if (text2.Contains("sfp") && num7 < 0)
			{
				num7 = tuple2.Item3;
			}
		}
		if (text == "qsfp" && num5 >= 0)
		{
			return num5;
		}
		if (text == "sfp28" && num6 >= 0)
		{
			return num6;
		}
		if (text == "sfp+" && num7 >= 0)
		{
			return num7;
		}
		if (text == "qsfp")
		{
			return (num5 >= 0) ? num5 : ((num6 >= 0) ? num6 : num7);
		}
		if (num7 >= 0)
		{
			return num7;
		}
		if (num6 >= 0)
		{
			return num6;
		}
		return -1;
	}

	private Dictionary<int, float> BuildSwitchPortTypeSpeedMap(NetworkSwitch sw)
	{
		if (_switchTypeSpeedMap.TryGetValue(sw, out var value))
		{
			return value;
		}
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		if ((UnityEngine.Object)(object)sw == (UnityEngine.Object)null || sw.cableLinkSwitchPorts == null)
		{
			_switchTypeSpeedMap[sw] = dictionary;
			return dictionary;
		}
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		foreach (CableLink item in (Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts)
		{
			if (!((UnityEngine.Object)(object)item == (UnityEngine.Object)null) && item.isSFPPort)
			{
				int sfpTypeSupported = item.sfpTypeSupported;
				dictionary2[sfpTypeSupported] = ((!dictionary2.TryGetValue(sfpTypeSupported, out var value2)) ? 1 : (value2 + 1));
			}
		}
		if (dictionary2.Count == 0)
		{
			_switchTypeSpeedMap[sw] = dictionary;
			return dictionary;
		}
		if (sw.switchType == 2)
		{
			foreach (int key in dictionary2.Keys)
			{
				dictionary[key] = 40f;
			}
		}
		else if (sw.switchType == 3 && dictionary2.Count >= 2)
		{
			List<KeyValuePair<int, int>> list = dictionary2.ToList();
			list.Sort((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => a.Value.CompareTo(b.Value));
			for (int num = 0; num < list.Count; num++)
			{
				dictionary[list[num].Key] = ((num == 0) ? 40f : 10f);
			}
		}
		else
		{
			foreach (int key2 in dictionary2.Keys)
			{
				dictionary[key2] = 10f;
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"  Switch {((UnityEngine.Object)((Component)sw).gameObject).name} (type {sw.switchType}) port-type ? speed map: {string.Join(", ", dictionary.Select((KeyValuePair<int, float> kv) => $"{kv.Key}?{kv.Value}"))}");
		_switchTypeSpeedMap[sw] = dictionary;
		return dictionary;
	}

	private float DeriveExpectedPortSpeed(NetworkSwitch sw, int portIndex, CableLink port)
	{
		if ((UnityEngine.Object)(object)sw == (UnityEngine.Object)null || (UnityEngine.Object)(object)port == (UnityEngine.Object)null)
		{
			return 0f;
		}
		// Ignore stale prefab/default connectionSpeed on empty SFP ports.
		if (port.connectionSpeed > 0.5f && (!port.isSFPPort || (UnityEngine.Object)(object)port.insertedSFP != (UnityEngine.Object)null))
		{
			return port.connectionSpeed;
		}
		Dictionary<int, float> dictionary = BuildSwitchPortTypeSpeedMap(sw);
		if (dictionary.TryGetValue(port.sfpTypeSupported, out var value))
		{
			return value;
		}
		return 0f;
	}

	private void AutoFillSfpModules()
	{
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null)
		{
			return;
		}
		MainGameManager val = UnityEngine.Object.FindObjectOfType<MainGameManager>();
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null || val.sfpPrefabs == null)
		{
			((MelonBase)this).LoggerInstance.Error("SFP prefabs unavailable");
			return;
		}
		List<(int, float, int, string)> list = BuildSfpPrefabList(val);
		if (list.Count == 0)
		{
			((MelonBase)this).LoggerInstance.Error("No SFP prefabs registered");
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (NetworkSwitch val2 in CollectRackSwitches())
		{
			if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null || val2.cableLinkSwitchPorts == null)
			{
				continue;
			}
			int num3 = -1;
			foreach (CableLink item2 in (Il2CppArrayBase<CableLink>)(object)val2.cableLinkSwitchPorts)
			{
				num3++;
				if ((UnityEngine.Object)(object)item2 == (UnityEngine.Object)null || !item2.isSFPPort || (UnityEngine.Object)(object)item2.insertedSFP != (UnityEngine.Object)null)
				{
					continue;
				}
				int sfpTypeSupported = item2.sfpTypeSupported;
				float num4 = DeriveExpectedPortSpeed(val2, num3, item2);
				try
				{
					int prefabIdx = FindBestSfpPrefab(list, sfpTypeSupported, num4);
					if (prefabIdx < 0)
					{
						((MelonBase)this).LoggerInstance.Warning($"  No SFP prefab for type={sfpTypeSupported} speed={num4}");
						num2++;
						continue;
					}
					GameObject val3 = ((Il2CppArrayBase<GameObject>)(object)val.sfpPrefabs)[prefabIdx];
					float targetSpeed = num4;
					string prefabName = ((UnityEngine.Object)val3).name.ToLowerInvariant();
					if (prefabName.Contains("rj45") && (targetSpeed <= 0f || targetSpeed > 10f))
						targetSpeed = 10f;
					if (_enableVerboseDiagnostics)
					{
						((MelonBase)this).LoggerInstance.Msg($"  Port {((UnityEngine.Object)((Component)val2).gameObject).name}[#{num3}] switchType={val2.switchType} portConnSpeed={item2.connectionSpeed} ? need speed={targetSpeed} ? prefab[{prefabIdx}] {((UnityEngine.Object)val3).name} (speed={list.Find(((int sfpType, float speed, int prefabIdx, string name) x) => x.prefabIdx == prefabIdx).Item2})");
					}
					GameObject val4 = UnityEngine.Object.Instantiate<GameObject>(val3, val.parentUsableObjects);
					SFPModule val5 = val4.GetComponent<SFPModule>() ?? val4.GetComponentInChildren<SFPModule>();
					if ((UnityEngine.Object)(object)val5 == (UnityEngine.Object)null)
					{
						UnityEngine.Object.Destroy((UnityEngine.Object)(object)val4);
						((MelonBase)this).LoggerInstance.Warning("  Spawned prefab has no SFPModule component");
						num2++;
					}
					else
					{
						val5.sfpType = sfpTypeSupported;
						val5.speed = ((targetSpeed > 0f) ? targetSpeed : val5.speed);
						if (targetSpeed > 0f)
							item2.connectionSpeed = targetSpeed;
						val5.InsertDirectlyIntoPort(item2);
						// Immediately overwrite whatever InsertDirectlyIntoPort reset the speed to.
						if (targetSpeed > 0f)
						{
							val5.speed = targetSpeed;
							item2.connectionSpeed = targetSpeed;
							// Also enforce each frame for 10 frames in case the game re-syncs async.
							MelonCoroutines.Start(EnforceSfpSpeedDeferred(val5, item2, targetSpeed, 10));
						}
						num++;
					}
				}
				catch (Exception ex)
				{
					((MelonBase)this).LoggerInstance.Error("  SFP insert failed on " + ((UnityEngine.Object)((Component)val2).gameObject).name + ": " + ex.Message);
					num2++;
				}
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Auto-SFP complete: filled {num}, skipped {num2}");
		ShowRackDetail();
	}

	private void AutoWireRack()
	{
		if (_disableAutoWireForDebug)
		{
			((MelonBase)this).LoggerInstance.Msg("AutoWireRack is disabled by debug flag");
			return;
		}

		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null)
		{
			return;
		}
		CablePositions val = UnityEngine.Object.FindObjectOfType<CablePositions>();
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
		{
			((MelonBase)this).LoggerInstance.Error("CablePositions not found");
			return;
		}
		List<UsableObject> rackUsableObjects = CollectRackUsableObjects();
		List<Server> rackServers = ExtractRackServers(rackUsableObjects);
		List<NetworkSwitch> rackSwitches = ExtractRackSwitches(rackUsableObjects);
		List<PatchPanel> rackPatchPanels = ExtractRackPatchPanels(rackUsableObjects);

		// Clear any ghost cable IDs that ServerInsertedInRack may have assigned to server ports.
		// These are unprotected positive IDs the game re-assigns between placement and auto-wire,
		// causing IsPortReadyForCable to falsely skip the port (most visible on the last placed server).
		int ghostsCleared = 0;
		foreach (Server ghostSrv in rackServers)
		{
			if ((UnityEngine.Object)(object)ghostSrv == (UnityEngine.Object)null || ghostSrv.cablelinks == null) continue;
			foreach (CableLink gcl in (Il2CppArrayBase<CableLink>)(object)ghostSrv.cablelinks)
			{
				if ((UnityEngine.Object)(object)gcl == (UnityEngine.Object)null) continue;
				int gid = gcl.cableIDsOnLink;
				if (gid > 0 && !_autoWireProtectedCableIds.Contains(gid))
				{
					gcl.cableIDsOnLink = -1;
					ghostsCleared++;
				}
			}
		}
		if (ghostsCleared > 0)
			((MelonBase)this).LoggerInstance.Msg($"[AutoWireRack] Cleared {ghostsCleared} ghost server port cable IDs before wiring");

		List<CableLink> list = new List<CableLink>();
		foreach (Server componentInChildren in rackServers)
		{
			if ((UnityEngine.Object)(object)componentInChildren == (UnityEngine.Object)null || componentInChildren.cablelinks == null)
			{
				continue;
			}
			bool flag = false;
			foreach (CableLink item2 in (Il2CppArrayBase<CableLink>)(object)componentInChildren.cablelinks)
			{
				if ((UnityEngine.Object)(object)item2 != (UnityEngine.Object)null && item2.cableIDsOnLink > 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				((MelonBase)this).LoggerInstance.Msg("  Skipping already-wired server " + ((UnityEngine.Object)((Component)componentInChildren).gameObject).name);
				continue;
			}
			foreach (CableLink item3 in (Il2CppArrayBase<CableLink>)(object)componentInChildren.cablelinks)
			{
				if (IsPortReadyForCable(item3))
				{
					list.Add(item3);
				}
			}
		}
		List<CableLink> list2 = new List<CableLink>();
		foreach (NetworkSwitch item4 in rackSwitches)
		{
			if ((UnityEngine.Object)(object)item4 == (UnityEngine.Object)null || item4.cableLinkSwitchPorts == null)
			{
				continue;
			}
			foreach (CableLink item5 in (Il2CppArrayBase<CableLink>)(object)item4.cableLinkSwitchPorts)
			{
				if (IsPortReadyForCable(item5))
				{
					list2.Add(item5);
				}
			}
		}
		List<(CableLink nearPort, CableLink farPort, int panelId)> patchPairs = new List<(CableLink nearPort, CableLink farPort, int panelId)>();
		HashSet<int> seenPatchPortIds = new HashSet<int>();
		foreach (PatchPanel pp in rackPatchPanels)
		{
			if ((UnityEngine.Object)(object)pp == (UnityEngine.Object)null || pp.cableLinkPorts == null)
				continue;
			int panelId = ((UnityEngine.Object)(object)pp).GetInstanceID();
			foreach (CableLink port in (Il2CppArrayBase<CableLink>)(object)pp.cableLinkPorts)
			{
				if ((UnityEngine.Object)(object)port == (UnityEngine.Object)null)
					continue;
				int portId = ((UnityEngine.Object)(object)port).GetInstanceID();
				if (seenPatchPortIds.Contains(portId))
					continue;
				CableLink paired = pp.GetPairedLink(port);
				if ((UnityEngine.Object)(object)paired == (UnityEngine.Object)null)
					continue;
				if (!IsPortReadyForCable(port) || !IsPortReadyForCable(paired))
					continue;
				seenPatchPortIds.Add(portId);
				seenPatchPortIds.Add(((UnityEngine.Object)(object)paired).GetInstanceID());
				patchPairs.Add((port, paired, panelId));
			}
		}
		List<int> patchPanelOrder = patchPairs.Select(((CableLink nearPort, CableLink farPort, int panelId) p) => p.panelId).Distinct().ToList();
		patchPanelOrder.Sort();
		List<int> orderedSwitchIds = list2
			.Select((CableLink p) => GetSwitchInstanceId(p))
			.Where((int id) => id > 0)
			.Distinct()
			.OrderBy((int id) => id)
			.ToList();
		Dictionary<int, int> switchUsage = new Dictionary<int, int>();
		((MelonBase)this).LoggerInstance.Msg($"Auto-wire: {list.Count} server ports, {list2.Count} switch ports");
		int num = 0;
		foreach (CableLink item6 in list)
		{
			bool routedViaPatchPanel = false;
			int serverPortIndex = GetServerPortIndex(item6);
			int preferredGroup = (serverPortIndex >= 0) ? (serverPortIndex % 2) : 0; // 0=A, 1=B
			int preferredPanelId = (patchPanelOrder.Count > 0 && serverPortIndex >= 0) ? patchPanelOrder[serverPortIndex % patchPanelOrder.Count] : -1;
			for (int pass = 0; pass < 2 && !routedViaPatchPanel; pass++)
			{
				int bestPatchIndex = -1;
				float bestPatchScore = float.MaxValue;
				for (int patchIndex = 0; patchIndex < patchPairs.Count; patchIndex++)
				{
					var pair = patchPairs[patchIndex];
					if ((UnityEngine.Object)(object)pair.nearPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)pair.farPort == (UnityEngine.Object)null)
						continue;
					if (preferredPanelId > 0)
					{
						bool panelMatch = pair.panelId == preferredPanelId;
						if ((pass == 0 && !panelMatch) || (pass == 1 && panelMatch))
							continue;
					}
					CableLink patchNear = pair.nearPort;
					CableLink patchFar = pair.farPort;
					float distNear = (((Component)patchNear).transform.position - ((Component)item6).transform.position).sqrMagnitude;
					float distFar = (((Component)patchFar).transform.position - ((Component)item6).transform.position).sqrMagnitude;
					if (distFar < distNear)
					{
						patchNear = pair.farPort;
						patchFar = pair.nearPort;
						distNear = distFar;
					}
					if (!PortsCompatible(item6, patchNear))
						continue;
					CableLink switchPortCandidate = PickBestSwitchPort(patchFar, list2, switchUsage, delegate(CableLink candidate)
					{
						if (!PortsCompatible(patchFar, candidate))
							return false;
						int swIdCandidate = GetSwitchInstanceId(candidate);
						if (orderedSwitchIds.Count >= 2 && swIdCandidate > 0)
						{
							int idx = orderedSwitchIds.IndexOf(swIdCandidate);
							if (idx >= 0)
							{
								bool groupMatch = (idx % 2) == preferredGroup;
								if (!groupMatch && pass == 0)
									return false;
							}
						}
						return true;
					});
					if ((UnityEngine.Object)(object)switchPortCandidate == (UnityEngine.Object)null)
						continue;
					float score = distNear;
					if (score < bestPatchScore)
					{
						bestPatchScore = score;
						bestPatchIndex = patchIndex;
					}
				}
				if (bestPatchIndex < 0)
					continue;
				var bestPair = patchPairs[bestPatchIndex];
				CableLink patchNear2 = bestPair.nearPort;
				CableLink patchFar2 = bestPair.farPort;
				float bestDistNear = (((Component)patchNear2).transform.position - ((Component)item6).transform.position).sqrMagnitude;
				float bestDistFar = (((Component)patchFar2).transform.position - ((Component)item6).transform.position).sqrMagnitude;
				if (bestDistFar < bestDistNear)
				{
					patchNear2 = bestPair.farPort;
					patchFar2 = bestPair.nearPort;
				}
				CableLink switchPort = PickBestSwitchPort(patchFar2, list2, switchUsage, delegate(CableLink candidate)
				{
					if (!PortsCompatible(patchFar2, candidate))
						return false;
					int swIdCandidate = GetSwitchInstanceId(candidate);
					if (orderedSwitchIds.Count >= 2 && swIdCandidate > 0)
					{
						int idx = orderedSwitchIds.IndexOf(swIdCandidate);
						if (idx >= 0)
						{
							bool groupMatch = (idx % 2) == preferredGroup;
							if (!groupMatch && pass == 0)
								return false;
						}
					}
					return true;
				});
				if ((UnityEngine.Object)(object)switchPort == (UnityEngine.Object)null)
					continue;
				try
				{
					string serverId = (((UnityEngine.Object)(object)item6.parentServer != (UnityEngine.Object)null) ? item6.parentServer.ServerID : "");
					List<Transform> serverPatchClips = FindCableClips(item6, patchNear2);
					List<Transform> switchPatchClips = FindCableClips(switchPort, patchFar2);
					bool ok1 = CreateCable(val, item6, patchNear2, serverPatchClips, CableLink.TypeOfLink.Server, CableLink.TypeOfLink.PatchPanel, serverId);
					bool ok2 = CreateCable(val, switchPort, patchFar2, switchPatchClips, CableLink.TypeOfLink.Switch, CableLink.TypeOfLink.PatchPanel, serverId);
					if (ok1 && ok2)
					{
						num += 2;
						routedViaPatchPanel = true;
						patchPairs.RemoveAt(bestPatchIndex);
						int swId = GetSwitchInstanceId(switchPort);
						if (swId > 0)
							switchUsage[swId] = switchUsage.TryGetValue(swId, out int used) ? used + 1 : 1;
						((MelonBase)this).LoggerInstance.Msg("  Wired server via patch panel");
						break;
					}
				}
				catch (Exception ex)
				{
					((MelonBase)this).LoggerInstance.Error("  Patch-panel wire failed: " + ex.Message);
				}
			}
			if (routedViaPatchPanel)
				continue;
			CableLink val3 = null;
			int value = -1;
			float bestDirectDist = float.MaxValue;
			for (int j = 0; j < list2.Count; j++)
			{
				CableLink val4 = list2[j];
				if (!IsPortReadyForCable(val4) || !PortsCompatible(item6, val4))
					continue;
				int swId = GetSwitchInstanceId(val4);
				if (orderedSwitchIds.Count >= 2 && swId > 0)
				{
					int idx = orderedSwitchIds.IndexOf(swId);
					if (idx >= 0 && (idx % 2) != preferredGroup)
						continue;
				}
				float d = (((Component)val4).transform.position - ((Component)item6).transform.position).sqrMagnitude;
				int used = (swId > 0 && switchUsage.TryGetValue(swId, out int cnt)) ? cnt : 0;
				float score = d + used * 25f;
				if (score < bestDirectDist)
				{
					bestDirectDist = score;
					val3 = val4;
					value = j;
				}
			}
			if ((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					CableLink val4 = list2[j];
					if (!IsPortReadyForCable(val4) || !PortsCompatible(item6, val4))
						continue;
					float d = (((Component)val4).transform.position - ((Component)item6).transform.position).sqrMagnitude;
					int swId = GetSwitchInstanceId(val4);
					int used = (swId > 0 && switchUsage.TryGetValue(swId, out int cnt)) ? cnt : 0;
					float score = d + used * 25f;
					if (score < bestDirectDist)
					{
						bestDirectDist = score;
						val3 = val4;
						value = j;
					}
				}
				if ((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null)
				{
					((MelonBase)this).LoggerInstance.Msg($"  No compatible switch port for server port (fibre={item6.isFibrePort}, sfp={item6.isSFPPort}, type={item6.sfpTypeSupported})");
					continue;
				}
			}
			try
			{
				string text = (((UnityEngine.Object)(object)item6.parentServer != (UnityEngine.Object)null) ? item6.parentServer.ServerID : "");
				List<Transform> list3 = FindCableClips(val3, item6);
				if (CreateCable(val, val3, item6, list3, CableLink.TypeOfLink.Switch, CableLink.TypeOfLink.Server, text))
				{
					num++;
					int swId2 = GetSwitchInstanceId(val3);
					if (swId2 > 0)
						switchUsage[swId2] = switchUsage.TryGetValue(swId2, out int used2) ? used2 + 1 : 1;
					((MelonBase)this).LoggerInstance.Msg($"  Wired switch -> server (match idx {value})");
				}
			}
			catch (Exception ex)
			{
				((MelonBase)this).LoggerInstance.Error("  Wire failed: " + ex.Message + "\n" + ex.StackTrace);
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Auto-wire complete: {num} connections made");
		SaveCableTopology();
		ShowRackDetail();
		static bool PortsCompatible(CableLink s, CableLink sw)
		{
			if ((UnityEngine.Object)(object)s == (UnityEngine.Object)null || (UnityEngine.Object)(object)sw == (UnityEngine.Object)null)
			{
				return false;
			}
			if (s.isFibrePort != sw.isFibrePort)
			{
				return false;
			}
			int num3 = (sw.isSFPPort ? sw.sfpTypeInserted : sw.sfpTypeSupported);
			int num4 = (s.isSFPPort ? s.sfpTypeInserted : s.sfpTypeSupported);
			if (num3 == 0 || num4 == 0)
			{
				return true;
			}
			return num3 == num4;
		}
	}

	private List<CableLink> GetRackRailClips()
	{
		if (_cachedRackRailClips != null && (UnityEngine.Object)(object)_cachedClipsRack == (UnityEngine.Object)(object)_selectedRack)
		{
			return _cachedRackRailClips;
		}
		List<CableLink> list = new List<CableLink>();
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null)
		{
			return list;
		}
		foreach (CableLink componentsInChild in ((Component)_selectedRack).GetComponentsInChildren<CableLink>(true))
		{
			if (!((UnityEngine.Object)(object)componentsInChild == (UnityEngine.Object)null) && !((UnityEngine.Object)(object)componentsInChild.parentServer != (UnityEngine.Object)null) && !((UnityEngine.Object)(object)componentsInChild.parentSwitch != (UnityEngine.Object)null) && !((UnityEngine.Object)(object)componentsInChild.parentPatchPanel != (UnityEngine.Object)null))
			{
				list.Add(componentsInChild);
			}
		}
		_cachedRackRailClips = list;
		_cachedClipsRack = _selectedRack;
		((MelonBase)this).LoggerInstance.Msg($"GetRackRailClips: found {list.Count} rail clips on {((UnityEngine.Object)((Component)_selectedRack).gameObject).name}");
		return list;
	}

	private List<Transform> FindCableClips(CableLink startPort, CableLink endPort)
	{
		List<Transform> list = new List<Transform>();
		List<CableLink> rackRailClips = GetRackRailClips();
		if (rackRailClips.Count == 0 || (UnityEngine.Object)(object)startPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)endPort == (UnityEngine.Object)null)
		{
			return list;
		}
		Transform rackTransform = ((Component)_selectedRack).transform;
		bool startRight = rackTransform.InverseTransformPoint(((Component)startPort).transform.position).x >= 0f;
		bool endRight = rackTransform.InverseTransformPoint(((Component)endPort).transform.position).x >= 0f;
		CableLink startClip = FindNearestClipOnSide(rackRailClips, ((Component)startPort).transform.position, startRight, rackTransform);
		CableLink endClip = FindNearestClipOnSide(rackRailClips, ((Component)endPort).transform.position, endRight, rackTransform);
		if ((UnityEngine.Object)(object)startClip != (UnityEngine.Object)null)
			list.Add(((Component)startClip).transform);
		if (startRight != endRight)
		{
			CableLink startBridge = FindTopClipOnSide(rackRailClips, startRight, rackTransform);
			CableLink endBridge = FindTopClipOnSide(rackRailClips, endRight, rackTransform);
			if ((UnityEngine.Object)(object)startBridge != (UnityEngine.Object)null && !list.Contains(((Component)startBridge).transform))
				list.Add(((Component)startBridge).transform);
			if ((UnityEngine.Object)(object)endBridge != (UnityEngine.Object)null && !list.Contains(((Component)endBridge).transform))
				list.Add(((Component)endBridge).transform);
		}
		if ((UnityEngine.Object)(object)endClip != (UnityEngine.Object)null && !list.Contains(((Component)endClip).transform))
			list.Add(((Component)endClip).transform);
		return list;
	}

	private static CableLink FindNearestClipOnSide(List<CableLink> clips, Vector3 targetPos, bool rightSide, Transform rackTransform)
	{
		CableLink best = null;
		float bestDist = float.MaxValue;
		foreach (CableLink clip in clips)
		{
			if ((UnityEngine.Object)(object)clip == (UnityEngine.Object)null)
				continue;
			bool clipRight = rackTransform.InverseTransformPoint(((Component)clip).transform.position).x >= 0f;
			if (clipRight != rightSide)
				continue;
			float d = (((Component)clip).transform.position - targetPos).sqrMagnitude;
			if (d < bestDist)
			{
				bestDist = d;
				best = clip;
			}
		}
		return best;
	}

	private static CableLink FindTopClipOnSide(List<CableLink> clips, bool rightSide, Transform rackTransform)
	{
		CableLink best = null;
		float bestY = float.MinValue;
		foreach (CableLink clip in clips)
		{
			if ((UnityEngine.Object)(object)clip == (UnityEngine.Object)null)
				continue;
			bool clipRight = rackTransform.InverseTransformPoint(((Component)clip).transform.position).x >= 0f;
			if (clipRight != rightSide)
				continue;
			float y = ((Component)clip).transform.position.y;
			if (y > bestY)
			{
				bestY = y;
				best = clip;
			}
		}
		return best;
	}

	private List<Transform> BuildRackExitPath(CableLink startPort)
	{
		List<Transform> path = new List<Transform>();
		if ((UnityEngine.Object)(object)startPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null)
			return path;
		List<CableLink> rackRailClips = GetRackRailClips();
		if (rackRailClips.Count == 0)
			return path;
		Transform rackTransform = ((Component)_selectedRack).transform;
		bool rightSide = rackTransform.InverseTransformPoint(((Component)startPort).transform.position).x >= 0f;
		CableLink startClip = FindNearestClipOnSide(rackRailClips, ((Component)startPort).transform.position, rightSide, rackTransform);
		CableLink topClip = FindTopClipOnSide(rackRailClips, rightSide, rackTransform);
		if ((UnityEngine.Object)(object)startClip != (UnityEngine.Object)null)
			path.Add(((Component)startClip).transform);
		if ((UnityEngine.Object)(object)topClip != (UnityEngine.Object)null && !path.Contains(((Component)topClip).transform))
			path.Add(((Component)topClip).transform);
		return path;
	}

	private List<CableLink> CollectOverheadClips()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (_cachedOverheadClips != null)
		{
			return _cachedOverheadClips;
		}
		List<CableLink> list = new List<CableLink>();
		foreach (CableLink item in UnityEngine.Object.FindObjectsOfType<CableLink>())
		{
			if (!((UnityEngine.Object)(object)item == (UnityEngine.Object)null) && !((UnityEngine.Object)(object)item.parentServer != (UnityEngine.Object)null) && !((UnityEngine.Object)(object)item.parentSwitch != (UnityEngine.Object)null) && !((UnityEngine.Object)(object)item.parentPatchPanel != (UnityEngine.Object)null) && !(((Component)item).transform.position.y < 2.5f) && !((UnityEngine.Object)(object)((Component)item).GetComponentInParent<Rack>() != (UnityEngine.Object)null))
			{
				list.Add(item);
			}
		}
		_cachedOverheadClips = list;
		((MelonBase)this).LoggerInstance.Msg($"CollectOverheadClips: found {list.Count} ceiling clips");
		return list;
	}

	private List<Transform> BuildOverheadPath(Vector3 from, Vector3 to)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		List<CableLink> list = CollectOverheadClips();
		if (list.Count == 0)
		{
			return new List<Transform>();
		}
		CableLink val = null;
		float num = float.MaxValue;
		Vector3 val2;
		foreach (CableLink item in list)
		{
			val2 = ((Component)item).transform.position - from;
			float sqrMagnitude = val2.sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				val = item;
			}
		}
		CableLink val3 = null;
		float num2 = float.MaxValue;
		foreach (CableLink item2 in list)
		{
			val2 = ((Component)item2).transform.position - to;
			float sqrMagnitude2 = val2.sqrMagnitude;
			if (sqrMagnitude2 < num2)
			{
				num2 = sqrMagnitude2;
				val3 = item2;
			}
		}
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null || (UnityEngine.Object)(object)val3 == (UnityEngine.Object)null)
		{
			return new List<Transform>();
		}
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)(object)val3)
		{
			return new List<Transform> { ((Component)val).transform };
		}
		List<Transform> list2 = new List<Transform>();
		HashSet<CableLink> hashSet = new HashSet<CableLink>();
		CableLink val4 = val;
		list2.Add(((Component)val4).transform);
		hashSet.Add(val4);
		int num3 = list.Count + 10;
		while ((UnityEngine.Object)(object)val4 != (UnityEngine.Object)(object)val3 && num3-- > 0)
		{
			val2 = ((Component)val4).transform.position - ((Component)val3).transform.position;
			float sqrMagnitude3 = val2.sqrMagnitude;
			CableLink val5 = null;
			float num4 = float.MaxValue;
			CableLink val6 = null;
			float num5 = float.MaxValue;
			CableLink val7 = null;
			float num6 = float.MaxValue;
			foreach (CableLink item3 in list)
			{
				if (!((UnityEngine.Object)(object)item3 == (UnityEngine.Object)null) && !hashSet.Contains(item3))
				{
					val2 = ((Component)item3).transform.position - ((Component)val4).transform.position;
					float sqrMagnitude4 = val2.sqrMagnitude;
					val2 = ((Component)item3).transform.position - ((Component)val3).transform.position;
					float sqrMagnitude5 = val2.sqrMagnitude;
					bool flag = sqrMagnitude5 < sqrMagnitude3;
					bool flag2 = sqrMagnitude4 <= 16f;
					if (flag2 && flag && sqrMagnitude4 < num4)
					{
						num4 = sqrMagnitude4;
						val5 = item3;
					}
					if (flag2 && sqrMagnitude4 < num5)
					{
						num5 = sqrMagnitude4;
						val6 = item3;
					}
					if (flag && sqrMagnitude4 < num6)
					{
						num6 = sqrMagnitude4;
						val7 = item3;
					}
				}
			}
			CableLink val8 = val5 ?? val6 ?? val7;
			if ((UnityEngine.Object)(object)val8 == (UnityEngine.Object)null)
			{
				break;
			}
			list2.Add(((Component)val8).transform);
			hashSet.Add(val8);
			val4 = val8;
		}
		if ((UnityEngine.Object)(object)val4 != (UnityEngine.Object)(object)val3 && !hashSet.Contains(val3))
		{
			list2.Add(((Component)val3).transform);
		}
		((MelonBase)this).LoggerInstance.Msg($"BuildOverheadPath: {list2.Count} waypoints from ({from.x:F1},{from.z:F1}) to ({to.x:F1},{to.z:F1})");
		return list2;
	}

	private void AutoWireToCustomer(int targetBaseId)
	{
		if (_disableAutoWireForDebug)
		{
			((MelonBase)this).LoggerInstance.Msg($"AutoWireToCustomer is disabled by debug flag (target {targetBaseId})");
			return;
		}
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null)
		{
			return;
		}
		int num = WireRackSwitchesToCustomer(_selectedRack, targetBaseId);
		((MelonBase)this).LoggerInstance.Msg($"AutoWireToCustomer complete: {num} cables");
		SaveCableTopology();
		ShowRackDetail();
	}

	private void InstallViaNPC()
	{
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null || _cartQty.Count == 0)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> item2 in _cartQty)
		{
			for (int i = 0; i < item2.Value; i++)
			{
				list.Add(item2.Key);
			}
		}
		list.Sort(delegate(int a, int b)
		{
			int num5 = ((_itemChoices[a].category == "server") ? 1 : 0);
			int value = ((_itemChoices[b].category == "server") ? 1 : 0);
			return num5.CompareTo(value);
		});
		int num = ((_selectedRack.positions != null) ? ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length : 0);
		HashSet<int> hashSet = new HashSet<int>();
		List<ItemChoice> list2 = new List<ItemChoice>();
		List<int> list3 = new List<int>();
		foreach (int item3 in list)
		{
			ItemChoice item = _itemChoices[item3];
			for (int num2 = num - item.sizeInU; num2 >= 0; num2--)
			{
				bool flag = false;
				for (int num3 = 0; num3 < item.sizeInU; num3++)
				{
					if (hashSet.Contains(num2 + num3))
					{
						flag = true;
						break;
					}
				}
				if (!flag && _selectedRack.IsPositionAvailable(num2, item.sizeInU))
				{
					list2.Add(item);
					list3.Add(num2);
					for (int num4 = 0; num4 < item.sizeInU; num4++)
					{
						hashSet.Add(num2 + num4);
					}
					break;
				}
			}
		}
		if (list2.Count > 0)
		{
			NPCBuilder.QueueBuildJobs(_selectedRack, list2, list3);
			((MelonBase)this).LoggerInstance.Msg($"Sent Greg to install {list2.Count} items");
		}
		_cartQty.Clear();
		ShowRackDetail();
	}

	private void InstallCartItems()
	{
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b89: Expected O, but got Unknown
		//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Expected O, but got Unknown
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Expected O, but got Unknown
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null || _cartQty.Count == 0)
		{
			return;
		}
		MainGameManager val = UnityEngine.Object.FindObjectOfType<MainGameManager>();
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> item in _cartQty)
		{
			for (int i = 0; i < item.Value; i++)
			{
				list.Add(item.Key);
			}
		}
		list.Sort(delegate(int a, int b)
		{
			int num17 = ((_itemChoices[a].category == "server") ? 1 : 0);
			int value2 = ((_itemChoices[b].category == "server") ? 1 : 0);
			return num17.CompareTo(value2);
		});
		int num = ((_selectedRack.positions != null) ? ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length : 0);
		int num2 = 0;
		bool flag = false;
		if (num >= 2 && (UnityEngine.Object)(object)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[0] != (UnityEngine.Object)null && (UnityEngine.Object)(object)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[num - 1] != (UnityEngine.Object)null)
		{
			float y = ((Component)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[0]).transform.position.y;
			float y2 = ((Component)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[num - 1]).transform.position.y;
			flag = y2 > y;
		}
		((MelonBase)this).LoggerInstance.Msg($"Rack orientation: top is {(flag ? "high" : "low")} index (slotCount={num})");
		HashSet<int> hashSet = new HashSet<int>();
		if (_selectedRack.positions != null)
		{
			for (int num3 = 0; num3 < num; num3++)
			{
				RackPosition val2 = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[num3];
				if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
				{
					continue;
				}
				UsableObject val3 = null;
				for (int num4 = 0; num4 < ((Component)val2).transform.childCount; num4++)
				{
					Transform child = ((Component)val2).transform.GetChild(num4);
					if (!((UnityEngine.Object)(object)child == (UnityEngine.Object)null))
					{
						val3 = ((Component)child).GetComponent<UsableObject>() ?? ((Component)child).GetComponentInChildren<UsableObject>();
						if ((UnityEngine.Object)(object)val3 != (UnityEngine.Object)null)
						{
							break;
						}
					}
				}
				if ((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null)
				{
					continue;
				}
				int num5 = ((val3.sizeInU <= 0) ? 1 : val3.sizeInU);
				int num6 = (flag ? (num3 - num5 + 1) : num3);
				for (int num7 = 0; num7 < num5; num7++)
				{
					int num8 = num6 + num7;
					if (num8 >= 0 && num8 < num)
					{
						hashSet.Add(num8);
					}
				}
			}
			if (_selectedRack.isPositionUsed != null)
			{
				for (int num9 = 0; num9 < ((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed).Length; num9++)
				{
					if (((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed)[num9] != 0)
					{
						hashSet.Add(num9);
					}
				}
			}
			((MelonBase)this).LoggerInstance.Msg($"Pre-occupied slot count: {hashSet.Count}");
		}
		foreach (int item2 in list)
		{
			ItemChoice itemChoice = _itemChoices[item2];
			int num10 = (flag ? (num - itemChoice.sizeInU) : 0);
			int num11 = ((!flag) ? (num - itemChoice.sizeInU) : 0);
			int num12 = ((!flag) ? 1 : (-1));
			for (int num13 = num10; flag ? (num13 >= num11) : (num13 <= num11); num13 += num12)
			{
				bool flag2 = false;
				for (int num14 = 0; num14 < itemChoice.sizeInU; num14++)
				{
					if (hashSet.Contains(num13 + num14))
					{
						flag2 = true;
						break;
					}
				}
				if (flag2 || !_selectedRack.IsPositionAvailable(num13, itemChoice.sizeInU))
				{
					continue;
				}
				int num15 = (flag ? (num13 + itemChoice.sizeInU - 1) : num13);
				RackPosition val4 = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[num15];
				if ((UnityEngine.Object)(object)val4 == (UnityEngine.Object)null)
				{
					continue;
				}
				// Guarantee a valid UID ? freshly spawned racks may have rackPosGlobalUID=0 on their positions,
				// which would cause save data with rackPositionUID=0, breaking the load screen indefinitely.
				if (val4.rackPosGlobalUID <= 0)
				{
					val.lastUsedRackPositionGlobalUID++;
					val4.rackPosGlobalUID = val.lastUsedRackPositionGlobalUID;
					((MelonBase)this).LoggerInstance.Msg($"  Assigned missing UID {val4.rackPosGlobalUID} to RackPosition slot {num15}");
				}
				try
				{
					string category = itemChoice.category;
					if (1 == 0)
					{
					}
					GameObject val5 = (GameObject)(category switch
					{
						"server" => val.GetServerPrefab(itemChoice.prefabIndex), 
						"switch" => val.GetSwitchPrefab(itemChoice.prefabIndex), 
						"patchpanel" => val.GetPatchPanelPrefab(itemChoice.prefabIndex), 
						_ => null, 
					});
					if (1 == 0)
					{
					}
					GameObject val6 = val5;
					if ((UnityEngine.Object)(object)val6 == (UnityEngine.Object)null)
					{
						continue;
					}
					// Instantiate under parentUsableObjects so the game's interaction/raycast system finds it.
					GameObject val7 = UnityEngine.Object.Instantiate<GameObject>(val6, val.parentUsableObjects);
					// Only scrub prefab-inherited cable state for servers/switches.
					// Patch panels were stable before broad scrub logic was added.
					if (itemChoice.category == "server" || itemChoice.category == "switch")
					{
						int clearedPrefabLinks = 0;
						foreach (CableLink cl in val7.GetComponentsInChildren<CableLink>(true))
						{
							if ((UnityEngine.Object)(object)cl == (UnityEngine.Object)null)
							{
								continue;
							}
							if (cl.cableIDsOnLink > 0)
							{
								clearedPrefabLinks++;
							}
							// Do not touch global CablePositions here; IDs on prefabs can collide with live cables.
							cl.cableIDsOnLink = -1;
						}
						if (clearedPrefabLinks > 0)
						{
							((MelonBase)this).LoggerInstance.Msg($"  Cleared inherited cable state on {clearedPrefabLinks} links ({itemChoice.category})");
						}
					}
					UsableObject component = val7.GetComponent<UsableObject>() ?? val7.GetComponentInChildren<UsableObject>(true);
					Vector3 val8 = Vector3.zero;
					Quaternion localRotation = Quaternion.identity;
					if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
					{
						val8 = component.secondPosition;
						localRotation = Quaternion.Euler(component.secondRotation);
						((MelonBase)this).LoggerInstance.Msg($"  {itemChoice.name} secondPos={val8} secondRot={component.secondRotation} pivotPos={component.offsetPivotPosition}");
					}
					// Parent under the RackPosition, then apply local offset from prefab.
					val7.transform.SetParent(((Component)val4).transform);
					val7.transform.localPosition = val8;
					val7.transform.localRotation = localRotation;
					Rigidbody component2 = val7.GetComponent<Rigidbody>();
					if ((UnityEngine.Object)(object)component2 != (UnityEngine.Object)null)
					{
						component2.isKinematic = true;
						component2.useGravity = false;
						component2.velocity = Vector3.zero;
						component2.angularVelocity = Vector3.zero;
					}
					UsableObject component3 = val7.GetComponent<UsableObject>() ?? val7.GetComponentInChildren<UsableObject>(true);
					if ((UnityEngine.Object)(object)component3 != (UnityEngine.Object)null)
					{
						component3.currentRackPosition = val4;
						component3.rackPositionUID = val4.rackPosGlobalUID;
						component3.objectInHands = false;
						component3.isDropAllowed = true;
						component3.isOnTrolley = false;
						component3.keepUpright = false;
						component3.storedPosition = num13;
						component3.sizeInU = itemChoice.sizeInU;
						int lastUsedRackPositionGlobalUID = val.lastUsedRackPositionGlobalUID;
						val.lastUsedRackPositionGlobalUID = lastUsedRackPositionGlobalUID + 1;
						((Interact)component3).uid = val.lastUsedRackPositionGlobalUID;
					}
					Server component4 = val7.GetComponent<Server>() ?? val7.GetComponentInChildren<Server>(true);
					if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
					{
						component4.ServerID = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
						component4.serverType = itemChoice.prefabIndex;
						((UsableObject)component4).prefabID = itemChoice.prefabIndex;
						if (component4.activeLinks != null)
						{
							component4.activeLinks.Clear();
						}
						if (component4.cablelinks != null)
						{
							foreach (CableLink item3 in (Il2CppArrayBase<CableLink>)(object)component4.cablelinks)
							{
								if ((UnityEngine.Object)(object)item3 != (UnityEngine.Object)null)
								{
									item3.cableIDsOnLink = -1;
									item3.parentServer = component4;
								}
							}
						}
					}
					NetworkSwitch component5 = val7.GetComponent<NetworkSwitch>() ?? val7.GetComponentInChildren<NetworkSwitch>(true);
					if ((UnityEngine.Object)(object)component5 != (UnityEngine.Object)null)
					{
						component5.switchId = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
						component5.switchType = itemChoice.prefabIndex;
						if (component5.cableLinkSwitchPorts != null)
						{
							foreach (CableLink item4 in (Il2CppArrayBase<CableLink>)(object)component5.cableLinkSwitchPorts)
							{
								if ((UnityEngine.Object)(object)item4 != (UnityEngine.Object)null)
								{
									item4.cableIDsOnLink = -1;
									item4.switchID = component5.switchId;
									item4.parentSwitch = component5;
								}
							}
						}
						component5.temporarilyDisconnectedCables = new Il2CppSystem.Collections.Generic.HashSet<int>();
					}
					PatchPanel component6 = val7.GetComponent<PatchPanel>() ?? val7.GetComponentInChildren<PatchPanel>(true);
					if ((UnityEngine.Object)(object)component6 != (UnityEngine.Object)null)
					{
						component6.patchPanelId = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
						component6.patchPanelType = itemChoice.prefabIndex;
						((UsableObject)component6).prefabID = itemChoice.prefabIndex;
					}
					try
					{
						bool scrubServerOrSwitchCables = false;
						if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
						{
							int timeToBrake = ((component4.timeToBrake > 0) ? component4.timeToBrake : 99999);
							int eolTime = ((component4.eolTime > 0) ? component4.eolTime : 99999);
							component4.timeToBrake = timeToBrake;
							component4.eolTime = eolTime;
							component4.isBroken = false;
							component4.isWarningCleared = true;
							ServerSaveData ssd = new ServerSaveData();
							ssd.serverID = component4.ServerID;
							ssd.serverType = component4.serverType;
							ssd.prefabID = ((UsableObject)component4).prefabID;
							ssd.rackPositionUID = val4.rackPosGlobalUID;
							ssd.position = val7.transform.position;
							ssd.rotation = val7.transform.rotation;
							ssd.customerID = 0;
							ssd.ip = "";
							ssd.isOn = false;
							ssd.isBroken = false;
							ssd.isWarningCleared = true;
							ssd.timeToBrake = timeToBrake;
							ssd.eolTime = eolTime;
							try { component4.Start(); } catch { }
							((MelonBase)this).LoggerInstance.Msg("Called Server.Start() before ServerInsertedInRack");
							LogPlacementDebugState("before-server-insert", component4, null);
							component4.ServerInsertedInRack(ssd);
							((MelonBase)this).LoggerInstance.Msg("Called ServerInsertedInRack");
							LogPlacementDebugState("after-server-insert", component4, null);
							component4.isOn = false;
							if (component4.activeLinks != null)
							{
								component4.activeLinks.Clear();
							}
							scrubServerOrSwitchCables = true;
						}
						if ((UnityEngine.Object)(object)component5 != (UnityEngine.Object)null)
						{
							int timeToBrake2 = ((component5.timeToBrake > 0) ? component5.timeToBrake : 99999);
							int eolTime2 = ((component5.eolTime > 0) ? component5.eolTime : 99999);
							component5.timeToBrake = timeToBrake2;
							component5.eolTime = eolTime2;
							component5.isBroken = false;
							component5.isWarningCleared = true;
							SwitchSaveData swsd = new SwitchSaveData();
							swsd.switchID = component5.switchId;
							swsd.switchType = component5.switchType;
							swsd.rackPositionUID = val4.rackPosGlobalUID;
							swsd.position = val7.transform.position;
							swsd.rotation = val7.transform.rotation;
							swsd.isOn = false;
							swsd.label = "";
							swsd.isBroken = false;
							swsd.isWarningCleared = true;
							swsd.timeToBrake = timeToBrake2;
							swsd.eolTime = eolTime2;
							LogPlacementDebugState("before-switch-insert", null, component5);
							component5.SwitchInsertedInRack(swsd);
							((MelonBase)this).LoggerInstance.Msg("Called SwitchInsertedInRack");
							LogPlacementDebugState("after-switch-insert", null, component5);
							component5.isOn = false;
							// Zero out stale prefab speeds on empty SFP ports immediately after insert.
							if (component5.cableLinkSwitchPorts != null)
							{
								foreach (CableLink sfpCl in (Il2CppArrayBase<CableLink>)(object)component5.cableLinkSwitchPorts)
								{
									if ((UnityEngine.Object)(object)sfpCl != (UnityEngine.Object)null && sfpCl.isSFPPort
										&& (UnityEngine.Object)(object)sfpCl.insertedSFP == (UnityEngine.Object)null)
										sfpCl.connectionSpeed = 0f;
								}
							}
							scrubServerOrSwitchCables = true;
						}
						if ((UnityEngine.Object)(object)component6 != (UnityEngine.Object)null)
						{
							PatchPanelSaveData ppsd = new PatchPanelSaveData();
							ppsd.patchPanelID = component6.patchPanelId;
							ppsd.patchPanelType = component6.patchPanelType;
							ppsd.rackPositionUID = val4.rackPosGlobalUID;
							ppsd.position = val7.transform.position;
							ppsd.rotation = val7.transform.rotation;
							component6.InsertedInRack(ppsd);
						}
						if (scrubServerOrSwitchCables)
						{
							MelonCoroutines.Start(ScrubPlacedDeviceCableIds(val7, 45));
							SanitizeGhostCableIDs();
						}
					}
					catch (Exception ex)
					{
						((MelonBase)this).LoggerInstance.Warning("InsertedInRack finalizer failed for " + itemChoice.name + ": " + ex.Message);
					}
					try
					{
						bool value = false;
						if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
						{
							value = component4.ValidateRackPosition();
							((MelonBase)this).LoggerInstance.Msg($"  ValidateRackPosition({itemChoice.name}) => {value}");
							LogPlacementDebugState("after-server-validate", component4, null);
						}
						else if ((UnityEngine.Object)(object)component5 != (UnityEngine.Object)null)
						{
							value = component5.ValidateRackPosition();
							((MelonBase)this).LoggerInstance.Msg($"  ValidateRackPosition({itemChoice.name}) => {value}");
							LogPlacementDebugState("after-switch-validate", null, component5);
						}
						else if ((UnityEngine.Object)(object)component6 != (UnityEngine.Object)null)
						{
							value = component6.ValidateRackPosition();
							((MelonBase)this).LoggerInstance.Msg($"  ValidateRackPosition({itemChoice.name}) => {value}");
						}
					}
					catch (Exception ex2)
					{
						((MelonBase)this).LoggerInstance.Warning("ValidateRackPosition check threw: " + ex2.Message);
					}
					MelonCoroutines.Start(EnableCollidersDelayed(val7));
					val4.SetUsed(true);
					_selectedRack.MarkPositionAsUsed(num13, itemChoice.sizeInU);
					for (int num16 = 0; num16 < itemChoice.sizeInU; num16++)
					{
						hashSet.Add(num13 + num16);
					}
					num2++;
					((MelonBase)this).LoggerInstance.Msg($"Placed {itemChoice.name} at U{num13 + 1}");
					break;
				}
				catch (Exception ex3)
				{
					((MelonBase)this).LoggerInstance.Error("Place failed: " + ex3.Message);
				}
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Installed {num2}/{list.Count} items");
		_cartQty.Clear();
		ShowRackDetail();
	}

	private void InstallRackAtMount(RackMount mount)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		if ((UnityEngine.Object)(object)mount == (UnityEngine.Object)null || mount.isRackInstantiated)
		{
			((MelonBase)this).LoggerInstance.Msg("Mount already has a rack or is null");
			return;
		}
		try
		{
			InteractObjectData val = new InteractObjectData((Interact)(object)mount);
			GameObject val2 = mount.InstantiateRack(val);
			if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
			{
				((MelonBase)this).LoggerInstance.Error("InstantiateRack returned null ? falling back to clone path");
				MelonCoroutines.Start(InstallAndClearRack(mount));
				return;
			}
			mount.isRackInstantiated = true;
			Rack val3 = val2.GetComponent<Rack>() ?? val2.GetComponentInChildren<Rack>();
			MainGameManager val4 = UnityEngine.Object.FindObjectOfType<MainGameManager>();
			if ((UnityEngine.Object)(object)val3 != (UnityEngine.Object)null && val3.positions != null && (UnityEngine.Object)(object)val4 != (UnityEngine.Object)null)
			{
				int num = 0;
				foreach (RackPosition item in (Il2CppArrayBase<RackPosition>)(object)val3.positions)
				{
					if (!((UnityEngine.Object)(object)item == (UnityEngine.Object)null) && item.rackPosGlobalUID <= 0)
					{
						int lastUsedRackPositionGlobalUID = val4.lastUsedRackPositionGlobalUID;
						val4.lastUsedRackPositionGlobalUID = lastUsedRackPositionGlobalUID + 1;
						item.rackPosGlobalUID = val4.lastUsedRackPositionGlobalUID;
						num++;
					}
				}
				((MelonBase)this).LoggerInstance.Msg($"InstantiateRack: assigned fresh UIDs to {num}/{((Il2CppArrayBase<RackPosition>)(object)val3.positions).Length} positions");
			}
			((MelonBase)this).LoggerInstance.Msg("Rack installed via InstantiateRack at " + ((UnityEngine.Object)((Component)mount).gameObject).name);
			_onDetailPage = false;
			ShowRackList();
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Error("InstantiateRack failed: " + ex.Message + " ? falling back");
			MelonCoroutines.Start(InstallAndClearRack(mount));
		}
	}

	private IEnumerator InstallFirstRack(RackMount mount)
	{
		HashSet<int> existing = new HashSet<int>();
		foreach (UsableObject uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
		{
			if ((UnityEngine.Object)(object)uo != (UnityEngine.Object)null)
			{
				existing.Add(((UnityEngine.Object)uo).GetInstanceID());
			}
		}
		Il2CppSystem.Collections.IEnumerator installRoutine = mount.InstallRack(true, 0);
		while (installRoutine.MoveNext())
		{
			yield return installRoutine.Current;
		}
		float startTime = Time.time;
		while (Time.time - startTime < 10f)
		{
			yield return null;
			foreach (UsableObject uo2 in UnityEngine.Object.FindObjectsOfType<UsableObject>(true))
			{
				if (!((UnityEngine.Object)(object)uo2 == (UnityEngine.Object)null) && !existing.Contains(((UnityEngine.Object)uo2).GetInstanceID()))
				{
					((Component)uo2).gameObject.SetActive(false);
					((Component)uo2).transform.position = new Vector3(0f, -500f, 0f);
				}
			}
			Rack rack = ((Component)mount).GetComponentInChildren<Rack>();
			if ((UnityEngine.Object)(object)rack != (UnityEngine.Object)null && rack.isPositionUsed != null)
			{
				for (int i = 0; i < ((Il2CppArrayBase<int>)(object)rack.isPositionUsed).Length; i++)
				{
					((Il2CppArrayBase<int>)(object)rack.isPositionUsed)[i] = 0;
				}
			}
		}
		((MelonBase)this).LoggerInstance.Msg("First rack installed and cleared");
		_onDetailPage = false;
		ShowRackList();
	}

	private IEnumerator InstallAndHideEquipment(RackMount mount)
	{
		HashSet<int> existing = new HashSet<int>();
		foreach (UsableObject uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
		{
			if ((UnityEngine.Object)(object)uo != (UnityEngine.Object)null)
			{
				existing.Add(((UnityEngine.Object)uo).GetInstanceID());
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Existing items before install: {existing.Count}");
		Il2CppSystem.Collections.IEnumerator installRoutine = mount.InstallRack(true, 0);
		while (installRoutine.MoveNext())
		{
			yield return installRoutine.Current;
		}
		float startTime = Time.time;
		int removed = 0;
		while (Time.time - startTime < 10f)
		{
			yield return null;
			foreach (UsableObject uo2 in UnityEngine.Object.FindObjectsOfType<UsableObject>(true))
			{
				if (!((UnityEngine.Object)(object)uo2 == (UnityEngine.Object)null) && !existing.Contains(((UnityEngine.Object)uo2).GetInstanceID()))
				{
					((Component)uo2).gameObject.SetActive(false);
					((Component)uo2).transform.position = new Vector3(0f, -500f, 0f);
					removed++;
				}
			}
			Rack rack = ((Component)mount).GetComponentInChildren<Rack>();
			if ((UnityEngine.Object)(object)rack != (UnityEngine.Object)null && rack.isPositionUsed != null)
			{
				for (int i = 0; i < ((Il2CppArrayBase<int>)(object)rack.isPositionUsed).Length; i++)
				{
					((Il2CppArrayBase<int>)(object)rack.isPositionUsed)[i] = 0;
				}
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Rack installed ? {removed} cheat items hidden");
		_onDetailPage = false;
		ShowRackList();
	}

	private IEnumerator InstallAndClearRack(RackMount mount)
	{
		Il2CppSystem.Collections.IEnumerator installRoutine = mount.InstallRack(true, 0);
		while (installRoutine.MoveNext())
		{
			yield return installRoutine.Current;
		}
		for (int frame = 0; frame < 30; frame++)
		{
			yield return null;
			ClearRackEquipment(mount);
		}
		for (int attempt = 0; attempt < 20; attempt++)
		{
			yield return (object)new WaitForSeconds(0.5f);
			if (!ClearRackEquipment(mount))
			{
				break;
			}
		}
		((MelonBase)this).LoggerInstance.Msg("Rack installed and cleared");
		_onDetailPage = false;
		ShowRackList();
	}

	private bool ClearRackEquipment(RackMount mount)
	{
		Rack componentInChildren = ((Component)mount).GetComponentInChildren<Rack>();
		if ((UnityEngine.Object)(object)componentInChildren == (UnityEngine.Object)null)
		{
			return false;
		}
		bool result = false;
		foreach (UsableObject componentsInChild in ((Component)componentInChildren).GetComponentsInChildren<UsableObject>())
		{
			if ((UnityEngine.Object)(object)componentsInChild != (UnityEngine.Object)null && (UnityEngine.Object)(object)((Component)componentsInChild).gameObject != (UnityEngine.Object)(object)((Component)componentInChildren).gameObject)
			{
				UnityEngine.Object.DestroyImmediate((UnityEngine.Object)(object)((Component)componentsInChild).gameObject);
				result = true;
			}
		}
		if (componentInChildren.isPositionUsed != null)
		{
			for (int i = 0; i < ((Il2CppArrayBase<int>)(object)componentInChildren.isPositionUsed).Length; i++)
			{
				((Il2CppArrayBase<int>)(object)componentInChildren.isPositionUsed)[i] = 0;
			}
		}
		if (componentInChildren.positions != null)
		{
			foreach (RackPosition item in (Il2CppArrayBase<RackPosition>)(object)componentInChildren.positions)
			{
				if ((UnityEngine.Object)(object)item == (UnityEngine.Object)null)
				{
					continue;
				}
				for (int num = ((Component)item).transform.childCount - 1; num >= 0; num--)
				{
					Transform child = ((Component)item).transform.GetChild(num);
					if ((UnityEngine.Object)(object)child != (UnityEngine.Object)null && (UnityEngine.Object)(object)((Component)child).GetComponent<UsableObject>() != (UnityEngine.Object)null)
					{
						UnityEngine.Object.DestroyImmediate((UnityEngine.Object)(object)((Component)child).gameObject);
						result = true;
					}
				}
			}
		}
		return result;
	}

	private void RemoveItemByAnchor(int anchorIdx, int size)
	{
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null || _selectedRack.positions == null || anchorIdx < 0 || anchorIdx >= ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length)
		{
			return;
		}
		RackPosition val = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[anchorIdx];
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
		{
			return;
		}
		UsableObject val2 = null;
		// Pass A: hierarchy walk under the target RackPosition
		for (int i = 0; i < ((Component)val).transform.childCount; i++)
		{
			Transform child = ((Component)val).transform.GetChild(i);
			if (!((UnityEngine.Object)(object)child == (UnityEngine.Object)null))
			{
				val2 = ((Component)child).GetComponent<UsableObject>() ?? ((Component)child).GetComponentInChildren<UsableObject>();
				if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
				{
					// IL2CPP polymorphism fallback: try concrete types directly
					Server _srv = ((Component)child).GetComponentInChildren<Server>();
					if ((UnityEngine.Object)(object)_srv != (UnityEngine.Object)null)
						val2 = (UsableObject)(object)_srv;
					else
					{
						NetworkSwitch _sw = ((Component)child).GetComponentInChildren<NetworkSwitch>();
						if ((UnityEngine.Object)(object)_sw != (UnityEngine.Object)null)
							val2 = (UsableObject)(object)_sw;
					}
				}
				if ((UnityEngine.Object)(object)val2 != (UnityEngine.Object)null)
				{
					break;
				}
			}
		}
		// Pass B: if the game reparented the item (Start() moved it to parentUsableObjects),
		// find it by currentRackPosition object-reference ? immune to UID integer collisions across racks.
		if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null && val.rackPosGlobalUID != 0)
		{
			foreach (UsableObject uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
			{
				if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null) continue;
				if ((UnityEngine.Object)(object)uo.currentRackPosition != (UnityEngine.Object)(object)val) continue;
				SFPModule sfp = ((Component)uo).GetComponent<SFPModule>();
				if ((UnityEngine.Object)(object)sfp != (UnityEngine.Object)null) continue;
				val2 = uo;
				break;
			}
		}
		if (size <= 0)
		{
			size = ((val2.sizeInU <= 0) ? 1 : val2.sizeInU);
		}
		int length = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length;
		bool flag = false;
		if (length >= 2 && (UnityEngine.Object)(object)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[0] != (UnityEngine.Object)null && (UnityEngine.Object)(object)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[length - 1] != (UnityEngine.Object)null)
		{
			float y = ((Component)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[0]).transform.position.y;
			float y2 = ((Component)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[length - 1]).transform.position.y;
			flag = y2 > y;
		}
		int num = (flag ? (anchorIdx - size + 1) : anchorIdx);
		if (num < 0)
		{
			num = 0;
		}
		if (num + size > length)
		{
			size = length - num;
		}
		try
		{
			CablePositions val3 = UnityEngine.Object.FindObjectOfType<CablePositions>();
			Server component = ((Component)val2).GetComponent<Server>();
			NetworkSwitch component2 = ((Component)val2).GetComponent<NetworkSwitch>();
			CableLink[] array = null;
			if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null && component.cablelinks != null)
			{
				Il2CppArrayBase<CableLink> _tmp1 = (Il2CppArrayBase<CableLink>)(object)component.cablelinks;
				array = _tmp1;
			}
			else if ((UnityEngine.Object)(object)component2 != (UnityEngine.Object)null && component2.cableLinkSwitchPorts != null)
			{
				Il2CppArrayBase<CableLink> _tmp2 = (Il2CppArrayBase<CableLink>)(object)component2.cableLinkSwitchPorts;
				array = _tmp2;
			}
			if (array != null && (UnityEngine.Object)(object)val3 != (UnityEngine.Object)null)
			{
				CableLink[] array2 = array;
				foreach (CableLink val4 in array2)
				{
					if ((UnityEngine.Object)(object)val4 == (UnityEngine.Object)null)
					{
						continue;
					}
					int cableIDsOnLink = val4.cableIDsOnLink;
					if (cableIDsOnLink > 0)
					{
						try
						{
							val3.RemovePosition(cableIDsOnLink);
						}
						catch
						{
						}
						val4.cableIDsOnLink = -1;
					}
				}
			}
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Warning("Cable cleanup warning: " + ex.Message);
		}
		_selectedRack.MarkPositionAsUnused(num, size);
		if (_selectedRack.isPositionUsed != null)
		{
			for (int k = 0; k < size; k++)
			{
				int num2 = num + k;
				if (num2 >= 0 && num2 < ((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed).Length)
				{
					((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed)[num2] = 0;
				}
			}
		}
		for (int l = 0; l < size; l++)
		{
			int num3 = num + l;
			if (num3 >= 0 && num3 < length && (UnityEngine.Object)(object)((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[num3] != (UnityEngine.Object)null)
			{
				try
				{
					((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[num3].SetUsed(false);
				}
				catch
				{
				}
			}
		}
		UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)val2).gameObject);
		((MelonBase)this).LoggerInstance.Msg($"Removed {((UnityEngine.Object)((Component)val2).gameObject).name} ? anchor U{anchorIdx + 1}, range U{num + 1}?U{num + size} ({size}U)");
	}

	private int RemoveAllItemsFromSelectedRack()
	{
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null || _selectedRack.positions == null)
		{
			return 0;
		}
		Dictionary<int, int> rpInstanceToSlot = new Dictionary<int, int>();
		Dictionary<int, int> rpUidToSlot = new Dictionary<int, int>();
		for (int i = 0; i < ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length; i++)
		{
			RackPosition rp = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[i];
			if ((UnityEngine.Object)(object)rp == (UnityEngine.Object)null)
				continue;
			rpInstanceToSlot[((Component)rp).gameObject.GetInstanceID()] = i;
			if (rp.rackPosGlobalUID > 0)
				rpUidToSlot[rp.rackPosGlobalUID] = i;
		}
		HashSet<int> seenAnchors = new HashSet<int>();
		List<(int anchor, int size)> toRemove = new List<(int anchor, int size)>();
		foreach (UsableObject uo in CollectRackUsableObjects())
		{
			if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null)
				continue;
			if ((UnityEngine.Object)(object)((Component)uo).GetComponent<SFPModule>() != (UnityEngine.Object)null)
				continue;
			int anchor = -1;
			if ((UnityEngine.Object)(object)uo.currentRackPosition != (UnityEngine.Object)null)
			{
				rpInstanceToSlot.TryGetValue(((Component)uo.currentRackPosition).gameObject.GetInstanceID(), out anchor);
			}
			if (anchor < 0 && uo.rackPositionUID > 0)
			{
				rpUidToSlot.TryGetValue(uo.rackPositionUID, out anchor);
			}
			if (anchor < 0)
			{
				anchor = uo.storedPosition;
			}
			if (anchor < 0 || anchor >= ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length)
				continue;
			if (!seenAnchors.Add(anchor))
				continue;
			int size = (uo.sizeInU > 0) ? uo.sizeInU : 1;
			toRemove.Add((anchor, size));
		}
		toRemove.Sort((a, b) => b.anchor.CompareTo(a.anchor));
		int removed = 0;
		foreach ((int anchor, int size) entry in toRemove)
		{
			try
			{
				RemoveItemByAnchor(entry.anchor, entry.size);
				removed++;
			}
			catch (Exception ex)
			{
				((MelonBase)this).LoggerInstance.Warning($"Bulk remove skipped anchor U{entry.anchor + 1}: {ex.Message}");
			}
		}
		return removed;
	}

	private static string DescribeUsableObject(UsableObject uo)
	{
		// Skip SFP modules ? they are inserted into switch ports and share rackPositionUID with their host switch
		SFPModule sfp = ((Component)uo).GetComponent<SFPModule>();
		if ((UnityEngine.Object)(object)sfp != (UnityEngine.Object)null)
			return null;
		Server srv = ((Component)uo).GetComponent<Server>();
		if ((UnityEngine.Object)(object)srv != (UnityEngine.Object)null)
		{
			string s = $"Server {uo.sizeInU}U";
			return srv.isBroken ? (s + " BROKEN") : (!srv.isOn ? (s + " [OFF]") : (s + " [ON]"));
		}
		NetworkSwitch sw = ((Component)uo).GetComponent<NetworkSwitch>();
		if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null)
		{
			int portCount = (sw.cableLinkSwitchPorts != null) ? ((Il2CppArrayBase<CableLink>)(object)sw.cableLinkSwitchPorts).Length : 0;
			string swLabel = (portCount > 0) ? $"Switch ({portCount}p)" : "Switch";
			return sw.isBroken ? (swLabel + " BROKEN") : (!sw.isOn ? (swLabel + " [OFF]") : (swLabel + " [ON]"));
		}
		PatchPanel pp = ((Component)uo).GetComponent<PatchPanel>();
		if ((UnityEngine.Object)(object)pp != (UnityEngine.Object)null)
			return "Patch Panel";
		return null;
	}

	private static string ClassifyLabel(string label)
	{
		if (label.Contains("Server")) return "Server";
		if (label.Contains("Switch")) return "Switch";
		if (label.Contains("Patch")) return "PatchPanel";
		return "Used";
	}

	private string FindEquipmentInSlot(int slotIndex)
	{
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null || _selectedRack.positions == null)
		{
			return "Unknown";
		}
		if (slotIndex >= ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length)
		{
			return "Unknown";
		}
		RackPosition val = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[slotIndex];
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
		{
			return "Unknown";
		}
		return SearchForEquipment(((Component)val).transform);
	}

	private string SearchForEquipment(Transform t)
	{
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (!((UnityEngine.Object)(object)child == (UnityEngine.Object)null))
			{
				Server component = ((Component)child).GetComponent<Server>();
				if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
				{
					string text = $"Server {((UsableObject)component).sizeInU}U";
					return component.isBroken ? (text + " BROKEN") : ((!component.isOn) ? (text + " [OFF]") : (text + " [ON]"));
				}
				NetworkSwitch component2 = ((Component)child).GetComponent<NetworkSwitch>();
				if ((UnityEngine.Object)(object)component2 != (UnityEngine.Object)null)
				{
					string text2 = "Switch";
					return component2.isBroken ? (text2 + " BROKEN") : ((!component2.isOn) ? (text2 + " [OFF]") : (text2 + " [ON]"));
				}
				PatchPanel component3 = ((Component)child).GetComponent<PatchPanel>();
				if ((UnityEngine.Object)(object)component3 != (UnityEngine.Object)null)
				{
					return "Patch Panel";
				}
				UsableObject component4 = ((Component)child).GetComponent<UsableObject>();
				if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
				{
					return $"{component4.objectInHandType} ({((UnityEngine.Object)child).name})";
				}
				string text3 = SearchForEquipment(child);
				if (text3 != "Occupied")
				{
					return text3;
				}
			}
		}
		return "Occupied";
	}

	private void BuildItemChoices()
	{
		_itemChoices.Clear();
		MainGameManager val = UnityEngine.Object.FindObjectOfType<MainGameManager>();
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
		{
			return;
		}
		if (val.serverPrefabs != null)
		{
			for (int i = 0; i < ((Il2CppArrayBase<GameObject>)(object)val.serverPrefabs).Length; i++)
			{
				GameObject val2 = ((Il2CppArrayBase<GameObject>)(object)val.serverPrefabs)[i];
				if (!((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null))
				{
					string text = val.ReturnServerNameFromType(i);
					if (string.IsNullOrEmpty(text))
					{
						text = ((UnityEngine.Object)val2).name;
					}
					Server component = val2.GetComponent<Server>();
					int num = (((UnityEngine.Object)(object)component != (UnityEngine.Object)null) ? ((UsableObject)component).sizeInU : 2);
					_itemChoices.Add(new ItemChoice
					{
						name = text,
						category = "server",
						prefabIndex = i,
						sizeInU = num
					});
					((MelonBase)this).LoggerInstance.Msg($"  Server type {i}: {text} ({num}U)");
				}
			}
		}
		if (val.switchesPrefabs != null)
		{
			for (int j = 0; j < ((Il2CppArrayBase<GameObject>)(object)val.switchesPrefabs).Length; j++)
			{
				GameObject val3 = ((Il2CppArrayBase<GameObject>)(object)val.switchesPrefabs)[j];
				if (!((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null))
				{
					string text2 = val.ReturnSwitchNameFromType(j);
					if (string.IsNullOrEmpty(text2))
					{
						text2 = ((UnityEngine.Object)val3).name;
					}
					NetworkSwitch component2 = val3.GetComponent<NetworkSwitch>();
					int num2 = 1;
					UsableObject component3 = val3.GetComponent<UsableObject>();
					if ((UnityEngine.Object)(object)component3 != (UnityEngine.Object)null)
					{
						num2 = component3.sizeInU;
					}
					_itemChoices.Add(new ItemChoice
					{
						name = text2,
						category = "switch",
						prefabIndex = j,
						sizeInU = num2
					});
					((MelonBase)this).LoggerInstance.Msg($"  Switch type {j}: {text2} ({num2}U)");
				}
			}
		}
		if (val.patchPanelsPrefabs != null)
		{
			for (int k = 0; k < ((Il2CppArrayBase<GameObject>)(object)val.patchPanelsPrefabs).Length; k++)
			{
				GameObject val4 = ((Il2CppArrayBase<GameObject>)(object)val.patchPanelsPrefabs)[k];
				if (!((UnityEngine.Object)(object)val4 == (UnityEngine.Object)null))
				{
					string text3 = ((UnityEngine.Object)val4).name.Replace("(Clone)", "").Trim();
					UsableObject component4 = val4.GetComponent<UsableObject>();
					int num3 = ((!((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)) ? 1 : component4.sizeInU);
					_itemChoices.Add(new ItemChoice
					{
						name = "Patch Panel (" + text3 + ")",
						category = "patchpanel",
						prefabIndex = k,
						sizeInU = num3
					});
					((MelonBase)this).LoggerInstance.Msg($"  PatchPanel type {k}: {text3} ({num3}U)");
				}
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Found {_itemChoices.Count} item types total");
	}

	private void PlaceItem(int slotIndex, int choiceIndex)
	{
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		if ((UnityEngine.Object)(object)_selectedRack == (UnityEngine.Object)null || choiceIndex < 0 || choiceIndex >= _itemChoices.Count)
		{
			return;
		}
		MainGameManager val = UnityEngine.Object.FindObjectOfType<MainGameManager>();
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
		{
			return;
		}
		ItemChoice itemChoice = _itemChoices[choiceIndex];
		if (!_selectedRack.IsPositionAvailable(slotIndex, itemChoice.sizeInU))
		{
			((MelonBase)this).LoggerInstance.Msg($"Slot U{slotIndex + 1} not available for {itemChoice.name}");
			return;
		}
		RackPosition val2 = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[slotIndex];
		if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
		{
			return;
		}
		try
		{
			string category = itemChoice.category;
			if (1 == 0)
			{
			}
			GameObject val3 = (GameObject)(category switch
			{
				"server" => val.GetServerPrefab(itemChoice.prefabIndex), 
				"switch" => val.GetSwitchPrefab(itemChoice.prefabIndex), 
				"patchpanel" => val.GetPatchPanelPrefab(itemChoice.prefabIndex), 
				_ => null, 
			});
			if (1 == 0)
			{
			}
			GameObject val4 = val3;
			if ((UnityEngine.Object)(object)val4 == (UnityEngine.Object)null)
			{
				return;
			}
			GameObject val5 = UnityEngine.Object.Instantiate<GameObject>(val4);
			foreach (Collider componentsInChild in val5.GetComponentsInChildren<Collider>())
			{
				if ((UnityEngine.Object)(object)componentsInChild != (UnityEngine.Object)null)
				{
					componentsInChild.enabled = false;
				}
			}
			val5.transform.SetParent(((Component)val2).transform);
			val5.transform.localPosition = Vector3.zero;
			val5.transform.localRotation = Quaternion.identity;
			Rigidbody component = val5.GetComponent<Rigidbody>();
			if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
			{
				component.isKinematic = true;
				component.useGravity = false;
				component.velocity = Vector3.zero;
				component.angularVelocity = Vector3.zero;
			}
			MelonCoroutines.Start(EnableCollidersDelayed(val5));
			UsableObject component2 = val5.GetComponent<UsableObject>();
			if ((UnityEngine.Object)(object)component2 != (UnityEngine.Object)null)
			{
				component2.currentRackPosition = val2;
				component2.rackPositionUID = val2.rackPosGlobalUID;
				component2.storedPosition = slotIndex;
				component2.sizeInU = itemChoice.sizeInU;
			}
			Server component3 = val5.GetComponent<Server>();
			if ((UnityEngine.Object)(object)component3 != (UnityEngine.Object)null)
			{
				component3.ServerID = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
				component3.serverType = itemChoice.prefabIndex;
				((UsableObject)component3).prefabID = itemChoice.prefabIndex;
			}
			NetworkSwitch component4 = val5.GetComponent<NetworkSwitch>();
			if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
			{
				component4.switchId = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
				component4.switchType = itemChoice.prefabIndex;
			}
			PatchPanel component5 = val5.GetComponent<PatchPanel>();
			if ((UnityEngine.Object)(object)component5 != (UnityEngine.Object)null)
			{
				component5.patchPanelId = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
				component5.patchPanelType = itemChoice.prefabIndex;
			}
			val2.SetUsed(true);
			_selectedRack.MarkPositionAsUsed(slotIndex, itemChoice.sizeInU);
			((MelonBase)this).LoggerInstance.Msg($"Placed {itemChoice.name} at U{slotIndex + 1}");
			ShowRackDetail();
		}
		catch (Exception ex)
		{
			((MelonBase)this).LoggerInstance.Error("Place failed: " + ex.Message);
		}
	}

	private void ClearContent()
	{
		foreach (GameObject uiRow in _uiRows)
		{
			if ((UnityEngine.Object)(object)uiRow != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy((UnityEngine.Object)(object)uiRow);
			}
		}
		_uiRows.Clear();
	}

	private void AddTitle(string text)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateRow(40f);
		TextMeshProUGUI val2 = val.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val2).text = text;
		((TMP_Text)val2).fontSize = 22f;
		((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
		((Graphic)val2).color = Color.white;
	}

	private void AddLabel(string text)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateRow(22f);
		TextMeshProUGUI val2 = val.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val2).text = text;
		((TMP_Text)val2).fontSize = 13f;
		((TMP_Text)val2).alignment = (TextAlignmentOptions)513;
		((Graphic)val2).color = new Color(0.7f, 0.7f, 0.7f);
		((TMP_Text)val2).enableWordWrapping = false;
	}

	private void AddColorLabel(string text, Color color)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateRow(24f);
		TextMeshProUGUI val2 = val.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val2).text = text;
		((TMP_Text)val2).fontSize = 14f;
		((TMP_Text)val2).alignment = (TextAlignmentOptions)513;
		((Graphic)val2).color = color;
		((TMP_Text)val2).enableWordWrapping = false;
	}

	private void AddDivider()
	{
		AddLabel("-----------------------------------------");
	}

	private void AddSpacer()
	{
		CreateRow(8f);
	}

	private void AddChoiceButton(string text, Color bgColor, Action onClick)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		AddClickableRow(text, bgColor, onClick);
	}

	private void AddQuantityRow(string itemName, int sizeInU, Color bgColor, int choiceIdx, int freeU)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Expected O, but got Unknown
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		int value;
		int num = (_cartQty.TryGetValue(choiceIdx, out value) ? value : 0);
		GameObject val = CreateRow(32f);
		Image val2 = val.AddComponent<Image>();
		((Graphic)val2).color = bgColor;
		HorizontalLayoutGroup val3 = val.AddComponent<HorizontalLayoutGroup>();
		((HorizontalOrVerticalLayoutGroup)val3).spacing = 4f;
		((HorizontalOrVerticalLayoutGroup)val3).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)val3).childForceExpandHeight = true;
		((HorizontalOrVerticalLayoutGroup)val3).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)val3).childControlHeight = true;
		RectOffset val4 = new RectOffset();
		val4.left = 8;
		val4.right = 6;
		((LayoutGroup)val3).padding = val4;
		GameObject val5 = new GameObject("Name");
		val5.transform.SetParent(val.transform, false);
		val5.AddComponent<RectTransform>();
		LayoutElement val6 = val5.AddComponent<LayoutElement>();
		val6.flexibleWidth = 1f;
		TextMeshProUGUI val7 = val5.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val7).text = $"{itemName}  ({sizeInU}U)";
		((TMP_Text)val7).fontSize = 14f;
		((TMP_Text)val7).alignment = (TextAlignmentOptions)513;
		((Graphic)val7).color = Color.white;
		((TMP_Text)val7).enableWordWrapping = false;
		int ci = choiceIdx;
		GameObject val8 = new GameObject("Minus");
		val8.transform.SetParent(val.transform, false);
		val8.AddComponent<RectTransform>();
		LayoutElement val9 = val8.AddComponent<LayoutElement>();
		val9.preferredWidth = 28f;
		val9.minWidth = 28f;
		Image val10 = val8.AddComponent<Image>();
		((Graphic)val10).color = (Color)((num > 0) ? (bgColor * 1.4f) : new Color(0.15f, 0.15f, 0.15f));
		Button val11 = val8.AddComponent<Button>();
		((UnityEvent)val11.onClick).AddListener((Action)delegate
		{
			if (_cartQty.TryGetValue(ci, out var value2) && value2 > 0)
			{
				value2--;
				if (value2 <= 0)
				{
					_cartQty.Remove(ci);
				}
				else
				{
					_cartQty[ci] = value2;
				}
				ShowRackDetail();
			}
		});
		ColorBlock colors = ((Selectable)val11).colors;
		colors.highlightedColor = bgColor * 1.8f;
		((Selectable)val11).colors = colors;
		TextMeshProUGUI val12 = new GameObject("T").AddComponent<TextMeshProUGUI>();
		((TMP_Text)val12).transform.SetParent(val8.transform, false);
		((TMP_Text)val12).text = "-";
		((TMP_Text)val12).fontSize = 18f;
		((TMP_Text)val12).alignment = (TextAlignmentOptions)514;
		((Graphic)val12).color = Color.white;
		RectTransform component = ((Component)val12).GetComponent<RectTransform>();
		component.anchorMin = Vector2.zero;
		component.anchorMax = Vector2.one;
		component.sizeDelta = Vector2.zero;
		GameObject val13 = new GameObject("Count");
		val13.transform.SetParent(val.transform, false);
		val13.AddComponent<RectTransform>();
		LayoutElement val14 = val13.AddComponent<LayoutElement>();
		val14.preferredWidth = 28f;
		val14.minWidth = 28f;
		TextMeshProUGUI val15 = val13.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val15).text = num.ToString();
		((TMP_Text)val15).fontSize = 14f;
		((TMP_Text)val15).alignment = (TextAlignmentOptions)514;
		((Graphic)val15).color = (Color)((num > 0) ? Color.white : new Color(0.4f, 0.4f, 0.4f));
		GameObject val16 = new GameObject("Plus");
		val16.transform.SetParent(val.transform, false);
		val16.AddComponent<RectTransform>();
		LayoutElement val17 = val16.AddComponent<LayoutElement>();
		val17.preferredWidth = 28f;
		val17.minWidth = 28f;
		Image val18 = val16.AddComponent<Image>();
		((Graphic)val18).color = bgColor * 1.4f;
		Button val19 = val16.AddComponent<Button>();
		int itemSize = sizeInU;
		((UnityEvent)val19.onClick).AddListener((Action)delegate
		{
			int num2 = 0;
			foreach (KeyValuePair<int, int> item in _cartQty)
			{
				if (item.Key >= 0 && item.Key < _itemChoices.Count)
				{
					num2 += _itemChoices[item.Key].sizeInU * item.Value;
				}
			}
			int num3 = (((UnityEngine.Object)(object)_selectedRack != (UnityEngine.Object)null && _selectedRack.positions != null) ? ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions).Length : 0);
			int num4 = 0;
			if ((UnityEngine.Object)(object)_selectedRack != (UnityEngine.Object)null && _selectedRack.isPositionUsed != null)
			{
				for (int i = 0; i < ((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed).Length; i++)
				{
					if (((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed)[i] != 0)
					{
						num4++;
					}
				}
			}
			int num5 = num3 - num4 - num2;
			if (itemSize <= num5)
			{
				int value2;
				int num6 = (_cartQty.TryGetValue(ci, out value2) ? value2 : 0);
				_cartQty[ci] = num6 + 1;
				ShowRackDetail();
			}
		});
		ColorBlock colors2 = ((Selectable)val19).colors;
		colors2.highlightedColor = bgColor * 1.8f;
		((Selectable)val19).colors = colors2;
		TextMeshProUGUI val20 = new GameObject("T").AddComponent<TextMeshProUGUI>();
		((TMP_Text)val20).transform.SetParent(val16.transform, false);
		((TMP_Text)val20).text = "+";
		((TMP_Text)val20).fontSize = 18f;
		((TMP_Text)val20).alignment = (TextAlignmentOptions)514;
		((Graphic)val20).color = Color.white;
		RectTransform component2 = ((Component)val20).GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.sizeDelta = Vector2.zero;
	}

	private void AddClickableRow(string text, Color bgColor, Action onClick)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateRow(32f);
		Image val2 = val.AddComponent<Image>();
		((Graphic)val2).color = bgColor;
		Button val3 = val.AddComponent<Button>();
		((UnityEvent)val3.onClick).AddListener(onClick);
		ColorBlock colors = ((Selectable)val3).colors;
		colors.highlightedColor = bgColor * 1.5f;
		colors.pressedColor = bgColor * 0.7f;
		((Selectable)val3).colors = colors;
		GameObject val4 = new GameObject("Text");
		val4.transform.SetParent(val.transform, false);
		RectTransform val5 = val4.AddComponent<RectTransform>();
		val5.anchorMin = Vector2.zero;
		val5.anchorMax = Vector2.one;
		val5.sizeDelta = Vector2.zero;
		TextMeshProUGUI val6 = val4.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val6).text = text;
		((TMP_Text)val6).fontSize = 14f;
		((TMP_Text)val6).alignment = (TextAlignmentOptions)513;
		((Graphic)val6).color = Color.white;
		((TMP_Text)val6).enableWordWrapping = false;
	}

	private GameObject CreateRow(float height)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject($"Row_{_uiRows.Count}");
		val.transform.SetParent(_contentParent, false);
		RectTransform val2 = val.AddComponent<RectTransform>();
		val2.sizeDelta = new Vector2(0f, height);
		LayoutElement val3 = val.AddComponent<LayoutElement>();
		val3.preferredHeight = height;
		val3.minHeight = height;
		_uiRows.Add(val);
		return val;
	}

	private Color GetColor(string type)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (1 == 0)
		{
		}
		Color result = (Color)(type switch
		{
			"Server" => new Color(0.4f, 0.7f, 1f), 
			"Switch" => new Color(1f, 0.8f, 0.3f), 
			"PatchPanel" => new Color(0.8f, 0.5f, 1f), 
			"Used" => new Color(0.6f, 0.6f, 0.6f), 
			_ => Color.white, 
		});
		if (1 == 0)
		{
		}
		return result;
	}

	public override void OnDeinitializeMelon()
	{
		((MelonBase)this).LoggerInstance.Msg("Rack Builder Mod unloaded.");
	}
}




