using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	private bool _integrated;

	// Fingerprint of rack instance IDs at last UI open — used to detect same-scene save reloads
	private HashSet<int> _lastRackIds = new HashSet<int>();

	private ComputerShop _shop;

	private GameObject _rackScreen;

	private Transform _contentParent;

	private readonly List<GameObject> _uiRows = new List<GameObject>();

	private bool _onDetailPage;

	private Rack _selectedRack;

	private List<Rack> _allRacks = new List<Rack>();

	private RackMount _pendingRemoveMount;

	private Dictionary<int, int> _cartQty = new Dictionary<int, int>();

	private List<(int sfpType, float speed, int prefabIdx, string name)> _sfpPrefabInfo;

	private Dictionary<NetworkSwitch, Dictionary<int, float>> _switchTypeSpeedMap = new Dictionary<NetworkSwitch, Dictionary<int, float>>();

	private List<CableLink> _cachedRackRailClips;

	private Rack _cachedClipsRack;

	private List<CableLink> _cachedOverheadClips;

	private List<ItemChoice> _itemChoices = new List<ItemChoice>();

	public override void OnInitializeMelon()
	{
		((MelonBase)this).LoggerInstance.Msg("Rack Builder Mod v1.0.1 initialized!");
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
		_cartQty.Clear();
		_itemChoices.Clear();
		_cachedRackRailClips = null;
		_cachedClipsRack = null;
		_cachedOverheadClips = null;
		_sfpPrefabInfo = null;
		_switchTypeSpeedMap.Clear();
	}

	public override void OnUpdate()
	{
		// Only used to drive initial integration; no background polling after that.
		if (!_integrated)
			TryIntegrate();
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

		// If the set has meaningfully changed, a save was likely reloaded
		bool changed = currentIds.Count != _lastRackIds.Count
			|| !currentIds.SetEquals(_lastRackIds);

		_lastRackIds = currentIds;

		if (changed && _lastRackIds.Count > 0)
		{
			((MelonBase)this).LoggerInstance.Msg("[RackBuilder] Rack set changed — resetting state for new save");
			bool wasShowing = _rackScreen != null && _rackScreen.activeSelf;
			FullReset();
			// Re-integrate immediately so the UI is available without waiting
			TryIntegrate();
			if (wasShowing)
				ShowRackList();
		}
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
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
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
						Rack r = componentInChildren;
						RackMount m = value;
						((UnityEvent)val11.onClick).AddListener((Action)delegate
						{
							if ((UnityEngine.Object)(object)r != (UnityEngine.Object)null)
							{
								_selectedRack = r;
								_onDetailPage = true;
								ShowRackDetail();
							}
						});
						EventTrigger val12 = val9.AddComponent<EventTrigger>();
						EventTrigger.Entry val13 = new EventTrigger.Entry();
						val13.eventID = (EventTriggerType)4;
						((UnityEvent<BaseEventData>)(object)val13.callback).AddListener((Action<BaseEventData>)delegate(BaseEventData data)
						{
							//IL_000c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0012: Invalid comparison between Unknown and I4
							PointerEventData val14 = ((Il2CppObjectBase)data).TryCast<PointerEventData>();
							if (val14 != null && (int)val14.button == 1)
							{
								_pendingRemoveMount = m;
								ShowRackList();
							}
						});
						val12.triggers.Add(val13);
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
		AddClickableRow("  YES — Remove rack + contents", new Color(0.5f, 0.15f, 0.15f), delegate
		{
			RackMount pendingRemoveMount2 = _pendingRemoveMount;
			_pendingRemoveMount = null;
			RemoveRackAtMount(pendingRemoveMount2);
			ShowRackList();
		});
		AddSpacer();
		AddClickableRow("  NO — Cancel", new Color(0.2f, 0.2f, 0.2f), delegate
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
			componentsInChild.cableIDsOnLink = 0;
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

	private void ShowRackDetail()
	{
		//IL_1343: Unknown result type (might be due to invalid IL or missing references)
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

		// Build rackPositionUID → slot-index lookup for Pass 2 (save-reload reliable mapping)
		Dictionary<int, int> rpUidToSlot = new Dictionary<int, int>();
		if (_selectedRack.positions != null)
		{
			for (int s = 0; s < num; s++)
			{
				RackPosition rp0 = ((Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)[s];
				if ((UnityEngine.Object)(object)rp0 != (UnityEngine.Object)null)
					rpUidToSlot[rp0.rackPosGlobalUID] = s;
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
					// Try UsableObject (polymorphic — covers Server/Switch/PatchPanel subclasses)
					uo = ((Component)child).GetComponent<UsableObject>();
					if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null)
						uo = ((Component)child).GetComponentInChildren<UsableObject>();
					if ((UnityEngine.Object)(object)uo != (UnityEngine.Object)null)
					{
						size = (uo.sizeInU > 0) ? uo.sizeInU : 1;
						label = DescribeUsableObject(uo);
						colorType = ClassifyLabel(label);
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
					string colorType2 = ClassifyLabel(label2);
					int size2 = (uo.sizeInU > 0) ? uo.sizeInU : 1;
					list.Add((slot, size2, label2, colorType2, uo));
					for (int u = slot; u < slot + size2 && u < num; u++) slotsFound.Add(u);
				}
			}
		}

		// Pass 3: scene-wide scan — catches items reparented outside the rack hierarchy by the save/load system.
		// Filter by rackPositionUID matching any position UID in the selected rack.
		{
			int p3uo = 0, p3srv = 0, p3sw = 0;
			foreach (UsableObject uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
			{
				if ((UnityEngine.Object)(object)uo == (UnityEngine.Object)null) continue;
				if (!rpUidToSlot.TryGetValue(uo.rackPositionUID, out int slot)) continue;
				if (slotsFound.Contains(slot)) continue;
				string label2 = DescribeUsableObject(uo);
				string colorType2 = ClassifyLabel(label2);
				int size2 = (uo.sizeInU > 0) ? uo.sizeInU : 1;
				list.Add((slot, size2, label2, colorType2, uo));
				for (int u = slot; u < slot + size2 && u < num; u++) slotsFound.Add(u);
				if (_selectedRack.isPositionUsed != null)
				{
					for (int u = slot; u < slot + size2 && u < ((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed).Length; u++)
					{
						if (((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed)[u] == 0)
						{
							((Il2CppArrayBase<int>)(object)_selectedRack.isPositionUsed)[u] = 1;
							num2++;
						}
					}
				}
				p3uo++;
			}
			// IL2CPP concrete-type fallbacks for save-loaded items whose UsableObject cast fails
			foreach (Server srv2 in UnityEngine.Object.FindObjectsOfType<Server>())
			{
				if ((UnityEngine.Object)(object)srv2 == (UnityEngine.Object)null) continue;
				UsableObject uoSrv = (UsableObject)(object)srv2;
				if (!rpUidToSlot.TryGetValue(uoSrv.rackPositionUID, out int slot)) continue;
				if (slotsFound.Contains(slot)) continue;
				int size2 = (uoSrv.sizeInU > 0) ? uoSrv.sizeInU : 3;
				string label2 = srv2.isBroken ? $"Server {size2}U BROKEN" : (srv2.isOn ? $"Server {size2}U [ON]" : $"Server {size2}U [OFF]");
				list.Add((slot, size2, label2, "Server", uoSrv));
				for (int u = slot; u < slot + size2 && u < num; u++) slotsFound.Add(u);
				p3srv++;
			}
			foreach (NetworkSwitch sw2 in UnityEngine.Object.FindObjectsOfType<NetworkSwitch>())
			{
				if ((UnityEngine.Object)(object)sw2 == (UnityEngine.Object)null) continue;
				UsableObject uoSw = (UsableObject)(object)sw2;
				if (!rpUidToSlot.TryGetValue(uoSw.rackPositionUID, out int slot)) continue;
				if (slotsFound.Contains(slot)) continue;
				string label2 = sw2.isBroken ? "Switch BROKEN" : (sw2.isOn ? "Switch [ON]" : "Switch [OFF]");
				list.Add((slot, 1, label2, "Switch", uoSw));
				slotsFound.Add(slot);
				p3sw++;
			}
			((MelonBase)this).LoggerInstance.Msg($"[ShowRackDetail] Pass1={list.Count - p3uo - p3srv - p3sw} | Pass2 hierarchy | Pass3 scene-wide: {p3uo} UO, {p3srv} srv, {p3sw} sw | total={list.Count}");
		}

		// Recompute free slots after possible reconciliation
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
			AddSpacer();
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
			AddClickableRow($"  CONFIRM — Install {num4} items ({num3}U) via Technician", new Color(0.15f, 0.4f, 0.15f), delegate
			{
				InstallViaNPC();
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
		if ((UnityEngine.Object)(object)_selectedRack != (UnityEngine.Object)null && _selectedRack.positions != null)
		{
			foreach (RackPosition item5 in (Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)
			{
				if ((UnityEngine.Object)(object)item5 == (UnityEngine.Object)null)
				{
					continue;
				}
				for (int num10 = 0; num10 < ((Component)item5).transform.childCount; num10++)
				{
					Transform child2 = ((Component)item5).transform.GetChild(num10);
					if ((UnityEngine.Object)(object)child2 == (UnityEngine.Object)null)
					{
						continue;
					}
					Server component = ((Component)child2).GetComponent<Server>();
					if (!((UnityEngine.Object)(object)component != (UnityEngine.Object)null))
					{
						continue;
					}
					bool flag = false;
					if (component.cablelinks != null)
					{
						foreach (CableLink item6 in (Il2CppArrayBase<CableLink>)(object)component.cablelinks)
						{
							if ((UnityEngine.Object)(object)item6 != (UnityEngine.Object)null && item6.cableIDsOnLink > 0)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						num8++;
					}
				}
			}
		}
		Il2CppArrayBase<NetworkSwitch> val3 = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>();
		int num11 = 0;
		foreach (NetworkSwitch item7 in val3)
		{
			if ((UnityEngine.Object)(object)item7 == (UnityEngine.Object)null)
			{
				continue;
			}
			num11++;
			if (item7.cableLinkSwitchPorts == null)
				continue;
			foreach (CableLink item8 in (Il2CppArrayBase<CableLink>)(object)item7.cableLinkSwitchPorts)
			{
				if ((UnityEngine.Object)(object)item8 != (UnityEngine.Object)null && item8.cableIDsOnLink == 0)
					num9++;
			}
		}
		if (num8 > 0 && num9 > 0)
		{
			AddColorLabel($"  Auto-Wire: {num8} unwired servers  |  {num9} free switch ports", Color.white);
			AddClickableRow("  AUTO-WIRE — Connect all servers to switches", new Color(0.1f, 0.3f, 0.4f), delegate
			{
				AutoWireRack();
			});
		}
		else if (num8 > 0)
		{
			AddColorLabel($"  {num8} unwired servers but no free switch ports", new Color(0.5f, 0.3f, 0.3f));
		}
		int num13 = 0;
		if ((UnityEngine.Object)(object)_selectedRack != (UnityEngine.Object)null && _selectedRack.positions != null)
		{
			foreach (RackPosition item9 in (Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)
			{
				if ((UnityEngine.Object)(object)item9 == (UnityEngine.Object)null)
				{
					continue;
				}
				for (int num14 = 0; num14 < ((Component)item9).transform.childCount; num14++)
				{
					Transform child3 = ((Component)item9).transform.GetChild(num14);
					NetworkSwitch val4 = ((child3 != null) ? ((Component)child3).GetComponentInChildren<NetworkSwitch>() : null);
					if ((UnityEngine.Object)(object)val4 == (UnityEngine.Object)null || val4.cableLinkSwitchPorts == null)
					{
						continue;
					}
					foreach (CableLink item10 in (Il2CppArrayBase<CableLink>)(object)val4.cableLinkSwitchPorts)
					{
						if ((UnityEngine.Object)(object)item10 != (UnityEngine.Object)null && item10.isSFPPort && (UnityEngine.Object)(object)item10.insertedSFP == (UnityEngine.Object)null)
						{
							num13++;
						}
					}
				}
			}
		}
		if (num13 > 0)
		{
			AddColorLabel($"  Auto-SFP: {num13} empty SFP/QSFP+ ports", Color.white);
			AddClickableRow("  AUTO-FILL — Insert modules into all empty SFP ports", new Color(0.1f, 0.3f, 0.4f), delegate
			{
				AutoFillSfpModules();
			});
		}
		int num15 = 0;
		if ((UnityEngine.Object)(object)_selectedRack != (UnityEngine.Object)null && _selectedRack.positions != null)
		{
			foreach (RackPosition item11 in (Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)
			{
				if ((UnityEngine.Object)(object)item11 == (UnityEngine.Object)null)
				{
					continue;
				}
				for (int num16 = 0; num16 < ((Component)item11).transform.childCount; num16++)
				{
					Transform child4 = ((Component)item11).transform.GetChild(num16);
					NetworkSwitch val5 = ((child4 != null) ? ((Component)child4).GetComponentInChildren<NetworkSwitch>() : null);
					if ((UnityEngine.Object)(object)val5 == (UnityEngine.Object)null || val5.cableLinkSwitchPorts == null)
					{
						continue;
					}
					bool flag2 = false;
					bool flag3 = false;
					foreach (CableLink item12 in (Il2CppArrayBase<CableLink>)(object)val5.cableLinkSwitchPorts)
					{
						if (!((UnityEngine.Object)(object)item12 == (UnityEngine.Object)null))
						{
							if (item12.cableIDsOnLink > 0)
							{
								flag2 = true;
							}
							if (item12.cableIDsOnLink <= 0 && (!item12.isSFPPort || (UnityEngine.Object)(object)item12.insertedSFP != (UnityEngine.Object)null))
							{
								flag3 = true;
							}
						}
					}
					if (flag3)
					{
						num15++;
					}
				}
			}
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
		AddColorLabel($"  Connect to Customer:  ({num15} switches need uplink)", Color.white);
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
			AddClickableRow($"    → {value3}  ({num21} free ports)", new Color(0.35f, 0.15f, 0.4f), delegate
			{
				AutoWireToCustomer(baseId);
			});
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
					((MelonBase)this).LoggerInstance.Msg($"  sfpPrefabs[{i}] {((UnityEngine.Object)val).name} — no SFPModule component");
				}
				else
				{
					((MelonBase)this).LoggerInstance.Msg($"  sfpPrefabs[{i}] {((UnityEngine.Object)val).name} — sfpType={val2.sfpType} speed={val2.speed}");
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
		for (int i = 0; i < list.Count; i++)
		{
			(int, float, int, string) tuple = list[i];
			if (tuple.Item1 == requiredType && tuple.Item2 > num2)
			{
				num2 = tuple.Item2;
				num = tuple.Item3;
			}
		}
		if (num >= 0)
		{
			return num;
		}
		if (list.Count == 0)
		{
			return -1;
		}
		string text = ((requiredSpeed >= 35f) ? "qsfp" : ((!(requiredSpeed >= 20f)) ? "sfp+" : "sfp28"));
		int num3 = -1;
		int num4 = -1;
		int num5 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			(int, float, int, string) tuple2 = list[j];
			string text2 = (tuple2.Item4 ?? "").ToLowerInvariant();
			if (text2.Contains("qsfp"))
			{
				if (num3 < 0)
				{
					num3 = tuple2.Item3;
				}
			}
			else if (text2.Contains("sfp28") || text2.Contains("sfp_28"))
			{
				if (num4 < 0)
				{
					num4 = tuple2.Item3;
				}
			}
			else if (text2.Contains("sfp") && num5 < 0)
			{
				num5 = tuple2.Item3;
			}
		}
		if (text == "qsfp" && num3 >= 0)
		{
			return num3;
		}
		if (text == "sfp28" && num4 >= 0)
		{
			return num4;
		}
		if (text == "sfp+" && num5 >= 0)
		{
			return num5;
		}
		if (text == "qsfp")
		{
			return (num3 >= 0) ? num3 : ((num4 >= 0) ? num4 : num5);
		}
		if (num5 >= 0)
		{
			return num5;
		}
		if (num4 >= 0)
		{
			return num4;
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
		((MelonBase)this).LoggerInstance.Msg($"  Switch {((UnityEngine.Object)((Component)sw).gameObject).name} (type {sw.switchType}) port-type → speed map: {string.Join(", ", dictionary.Select((KeyValuePair<int, float> kv) => $"{kv.Key}→{kv.Value}"))}");
		_switchTypeSpeedMap[sw] = dictionary;
		return dictionary;
	}

	private float DeriveExpectedPortSpeed(NetworkSwitch sw, int portIndex, CableLink port)
	{
		if ((UnityEngine.Object)(object)port != (UnityEngine.Object)null && port.connectionSpeed > 0.5f)
		{
			return port.connectionSpeed;
		}
		if ((UnityEngine.Object)(object)sw == (UnityEngine.Object)null || (UnityEngine.Object)(object)port == (UnityEngine.Object)null)
		{
			return 0f;
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
		foreach (RackPosition item in (Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)
		{
			if ((UnityEngine.Object)(object)item == (UnityEngine.Object)null)
			{
				continue;
			}
			for (int i = 0; i < ((Component)item).transform.childCount; i++)
			{
				Transform child = ((Component)item).transform.GetChild(i);
				NetworkSwitch val2 = ((child != null) ? ((Component)child).GetComponentInChildren<NetworkSwitch>() : null);
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
						((MelonBase)this).LoggerInstance.Msg($"  Port {((UnityEngine.Object)((Component)val2).gameObject).name}[#{num3}] switchType={val2.switchType} portConnSpeed={item2.connectionSpeed} → need speed={num4} → prefab[{prefabIdx}] {((UnityEngine.Object)val3).name} (speed={list.Find(((int sfpType, float speed, int prefabIdx, string name) x) => x.prefabIdx == prefabIdx).Item2})");
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
							val5.speed = ((num4 > 0f) ? num4 : val5.speed);
							val5.InsertDirectlyIntoPort(item2);
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
		}
		((MelonBase)this).LoggerInstance.Msg($"Auto-SFP complete: filled {num}, skipped {num2}");
		ShowRackDetail();
	}

	private void AutoWireRack()
	{
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
		List<CableLink> list = new List<CableLink>();
		foreach (RackPosition item in (Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)
		{
			if ((UnityEngine.Object)(object)item == (UnityEngine.Object)null)
			{
				continue;
			}
			for (int i = 0; i < ((Component)item).transform.childCount; i++)
			{
				Transform child = ((Component)item).transform.GetChild(i);
				if ((UnityEngine.Object)(object)child == (UnityEngine.Object)null)
				{
					continue;
				}
				Server componentInChildren = ((Component)child).GetComponentInChildren<Server>();
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
					if (!((UnityEngine.Object)(object)item3 == (UnityEngine.Object)null) && item3.cableIDsOnLink == 0 && (!item3.isSFPPort || !((UnityEngine.Object)(object)item3.insertedSFP == (UnityEngine.Object)null)))
					{
						list.Add(item3);
					}
				}
			}
		}
		List<CableLink> list2 = new List<CableLink>();
		Il2CppArrayBase<NetworkSwitch> val2 = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>();
		foreach (NetworkSwitch item4 in val2)
		{
			if ((UnityEngine.Object)(object)item4 == (UnityEngine.Object)null || item4.cableLinkSwitchPorts == null)
			{
				continue;
			}
			foreach (CableLink item5 in (Il2CppArrayBase<CableLink>)(object)item4.cableLinkSwitchPorts)
			{
				if (!((UnityEngine.Object)(object)item5 == (UnityEngine.Object)null) && item5.cableIDsOnLink == 0 && (!item5.isSFPPort || !((UnityEngine.Object)(object)item5.insertedSFP == (UnityEngine.Object)null)))
				{
					list2.Add(item5);
				}
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Auto-wire: {list.Count} server ports, {list2.Count} switch ports");
		int num = 0;
		foreach (CableLink item6 in list)
		{
			CableLink val3 = null;
			int value = -1;
			for (int j = 0; j < list2.Count; j++)
			{
				CableLink val4 = list2[j];
				if (!((UnityEngine.Object)(object)val4 == (UnityEngine.Object)null) && val4.cableIDsOnLink == 0 && PortsCompatible(item6, val4))
				{
					val3 = val4;
					value = j;
					break;
				}
			}
			if ((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null)
			{
				((MelonBase)this).LoggerInstance.Msg($"  No compatible switch port for server port (fibre={item6.isFibrePort}, sfp={item6.isSFPPort}, type={item6.sfpTypeSupported})");
				continue;
			}
			try
			{
				int num2 = val.CreateNewCable();
				string text = (((UnityEngine.Object)(object)item6.parentServer != (UnityEngine.Object)null) ? item6.parentServer.ServerID : "");
				val.AssignNewPosition(num2, ((Component)val3).transform, true, false, (CableLink.TypeOfLink)2, "");
				List<Transform> list3 = FindCableClips(val3, item6);
				foreach (Transform item7 in list3)
				{
					val.AssignNewPosition(num2, item7, false, false, (CableLink.TypeOfLink)1, text);
				}
				val.AssignNewPosition(num2, ((Component)item6).transform, false, true, (CableLink.TypeOfLink)1, text);
				val.GenerateFinalPath(num2);
				val.RedrawCable(num2);
				val3.cableIDsOnLink = num2;
				item6.cableIDsOnLink = num2;
				num++;
				((MelonBase)this).LoggerInstance.Msg($"  Wired cable {num2}: switch → server (match idx {value})");
			}
			catch (Exception ex)
			{
				((MelonBase)this).LoggerInstance.Error("  Wire failed: " + ex.Message + "\n" + ex.StackTrace);
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"Auto-wire complete: {num} connections made");
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
			if (num3 == 0 && num4 == 0)
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
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		List<Transform> list = new List<Transform>();
		List<CableLink> rackRailClips = GetRackRailClips();
		if (rackRailClips.Count == 0 || (UnityEngine.Object)(object)startPort == (UnityEngine.Object)null || (UnityEngine.Object)(object)endPort == (UnityEngine.Object)null)
		{
			return list;
		}
		Transform transform = ((Component)_selectedRack).transform;
		float x = transform.InverseTransformPoint(((Component)startPort).transform.position).x;
		bool flag = x >= 0f;
		CableLink val = null;
		CableLink val2 = null;
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		foreach (CableLink item in rackRailClips)
		{
			float x2 = transform.InverseTransformPoint(((Component)item).transform.position).x;
			bool flag2 = x2 >= 0f;
			if (flag2 == flag)
			{
				Vector3 val3 = ((Component)item).transform.position - ((Component)startPort).transform.position;
				float sqrMagnitude = val3.sqrMagnitude;
				val3 = ((Component)item).transform.position - ((Component)endPort).transform.position;
				float sqrMagnitude2 = val3.sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					val = item;
				}
				if (sqrMagnitude2 < num2)
				{
					num2 = sqrMagnitude2;
					val2 = item;
				}
			}
		}
		if ((UnityEngine.Object)(object)val != (UnityEngine.Object)null)
		{
			list.Add(((Component)val).transform);
		}
		if ((UnityEngine.Object)(object)val2 != (UnityEngine.Object)null && (UnityEngine.Object)(object)val2 != (UnityEngine.Object)(object)val)
		{
			list.Add(((Component)val2).transform);
		}
		return list;
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
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
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
		CustomerBase val2 = null;
		foreach (CustomerBase item in UnityEngine.Object.FindObjectsOfType<CustomerBase>())
		{
			if ((UnityEngine.Object)(object)item != (UnityEngine.Object)null && item.customerBaseID == targetBaseId)
			{
				val2 = item;
				break;
			}
		}
		if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null || val2.cableLinks == null)
		{
			((MelonBase)this).LoggerInstance.Error($"CustomerBase {targetBaseId} not found");
			ShowRackDetail();
			return;
		}
		List<CableLink> list = new List<CableLink>();
		foreach (CableLink item2 in (Il2CppArrayBase<CableLink>)(object)val2.cableLinks)
		{
			if (!((UnityEngine.Object)(object)item2 == (UnityEngine.Object)null) && item2.cableIDsOnLink <= 0)
			{
				list.Add(item2);
			}
		}
		if (list.Count == 0)
		{
			((MelonBase)this).LoggerInstance.Msg("No free customer ports on this base");
			ShowRackDetail();
			return;
		}
		Vector3 position = ((Component)val2).transform.position;
		List<CableLink> list2 = new List<CableLink>();
		if (_selectedRack.positions != null)
		{
			foreach (RackPosition item3 in (Il2CppArrayBase<RackPosition>)(object)_selectedRack.positions)
			{
				if ((UnityEngine.Object)(object)item3 == (UnityEngine.Object)null)
				{
					continue;
				}
				for (int i = 0; i < ((Component)item3).transform.childCount; i++)
				{
					Transform child = ((Component)item3).transform.GetChild(i);
					NetworkSwitch val3 = ((child != null) ? ((Component)child).GetComponentInChildren<NetworkSwitch>() : null);
					if ((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null || val3.cableLinkSwitchPorts == null)
					{
						continue;
					}
					CableLink val4 = null;
					foreach (CableLink item4 in (Il2CppArrayBase<CableLink>)(object)val3.cableLinkSwitchPorts)
					{
						if ((UnityEngine.Object)(object)item4 == (UnityEngine.Object)null || item4.cableIDsOnLink > 0 || (item4.isSFPPort && (UnityEngine.Object)(object)item4.insertedSFP == (UnityEngine.Object)null))
						{
							continue;
						}
						bool flag = false;
						foreach (CableLink item5 in list)
						{
							if ((UnityEngine.Object)(object)item5 == (UnityEngine.Object)null || item5.cableIDsOnLink > 0 || item5.isFibrePort != item4.isFibrePort)
							{
								continue;
							}
							flag = true;
							break;
						}
						if (!flag)
						{
							continue;
						}
						val4 = item4;
						break;
					}
					if ((UnityEngine.Object)(object)val4 != (UnityEngine.Object)null)
					{
						list2.Add(val4);
					}
				}
			}
		}
		if (list2.Count == 0)
		{
			((MelonBase)this).LoggerInstance.Msg("No compatible switch ports for this customer's port types");
			ShowRackDetail();
			return;
		}
		((MelonBase)this).LoggerInstance.Msg($"AutoWireToCustomer: {list2.Count} switch ports, {list.Count} customer ports");
		Vector3 position2 = ((Component)_selectedRack).transform.position;
		List<Transform> list3 = BuildOverheadPath(position2, position);
		List<CableLink> rackRailClips = GetRackRailClips();
		int num = 0;
		foreach (CableLink item6 in list2)
		{
			CableLink val5 = null;
			foreach (CableLink item7 in list)
			{
				if ((UnityEngine.Object)(object)item7 == (UnityEngine.Object)null || item7.cableIDsOnLink > 0 || item7.isFibrePort != item6.isFibrePort)
				{
					continue;
				}
				val5 = item7;
				break;
			}
			if ((UnityEngine.Object)(object)val5 == (UnityEngine.Object)null)
			{
				continue;
			}
			try
			{
				int num2 = val.CreateNewCable();
				val.AssignNewPosition(num2, ((Component)item6).transform, true, false, (CableLink.TypeOfLink)2, "");
				CableLink val6 = null;
				float num3 = float.MaxValue;
				foreach (CableLink item8 in rackRailClips)
				{
					if (!((UnityEngine.Object)(object)item8 == (UnityEngine.Object)null))
					{
						Vector3 val7 = ((Component)item8).transform.position - ((Component)item6).transform.position;
						float sqrMagnitude = val7.sqrMagnitude;
						if (sqrMagnitude < num3)
						{
							num3 = sqrMagnitude;
							val6 = item8;
						}
					}
				}
				if ((UnityEngine.Object)(object)val6 != (UnityEngine.Object)null)
				{
					val.AssignNewPosition(num2, ((Component)val6).transform, false, false, (CableLink.TypeOfLink)1, "");
				}
				foreach (Transform item9 in list3)
				{
					val.AssignNewPosition(num2, item9, false, false, (CableLink.TypeOfLink)1, "");
				}
				val.AssignNewPosition(num2, ((Component)val5).transform, false, true, (CableLink.TypeOfLink)2, "");
				val.GenerateFinalPath(num2);
				val.RedrawCable(num2);
				item6.cableIDsOnLink = num2;
				val5.cableIDsOnLink = num2;
				num++;
			}
			catch (Exception ex)
			{
				((MelonBase)this).LoggerInstance.Error("  Customer wire failed: " + ex.Message);
			}
		}
		((MelonBase)this).LoggerInstance.Msg($"AutoWireToCustomer complete: {num} cables routed through {list3.Count} overhead waypoints");
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
					GameObject val7 = UnityEngine.Object.Instantiate<GameObject>(val6, val.parentUsableObjects);
					UsableObject component = val7.GetComponent<UsableObject>();
					Vector3 val8 = Vector3.zero;
					Quaternion localRotation = Quaternion.identity;
					if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
					{
						val8 = component.secondPosition;
						localRotation = Quaternion.Euler(component.secondRotation);
						((MelonBase)this).LoggerInstance.Msg($"  {itemChoice.name} secondPos={val8} secondRot={component.secondRotation} pivotPos={component.offsetPivotPosition}");
					}
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
					UsableObject component3 = val7.GetComponent<UsableObject>();
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
					Server component4 = val7.GetComponent<Server>();
					if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
					{
						component4.ServerID = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
						component4.serverType = itemChoice.prefabIndex;
						((UsableObject)component4).prefabID = itemChoice.prefabIndex;
						component4.hasInitialized = true;
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
									item3.cableIDsOnLink = 0;
									item3.parentServer = component4;
								}
							}
						}
					}
					NetworkSwitch component5 = val7.GetComponent<NetworkSwitch>();
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
									item4.cableIDsOnLink = 0;
									item4.switchID = component5.switchId;
									item4.parentSwitch = component5;
								}
							}
						}
					}
					PatchPanel component6 = val7.GetComponent<PatchPanel>();
					if ((UnityEngine.Object)(object)component6 != (UnityEngine.Object)null)
					{
						component6.patchPanelId = "Mod_" + Guid.NewGuid().ToString().Substring(0, 8);
						component6.patchPanelType = itemChoice.prefabIndex;
					}
					try
					{
						if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
						{
							int timeToBrake = ((component4.timeToBrake > 0) ? component4.timeToBrake : 99999);
							int eolTime = ((component4.eolTime > 0) ? component4.eolTime : 99999);
							component4.timeToBrake = timeToBrake;
							component4.eolTime = eolTime;
							component4.isBroken = false;
							component4.isWarningCleared = true;
							ServerSaveData val9 = new ServerSaveData();
							val9.serverID = component4.ServerID;
							val9.serverType = component4.serverType;
							val9.prefabID = ((UsableObject)component4).prefabID;
							val9.rackPositionUID = val4.rackPosGlobalUID;
							val9.position = val7.transform.position;
							val9.rotation = val7.transform.rotation;
							val9.customerID = 0;
							val9.ip = "";
							val9.isOn = false;
							val9.isBroken = false;
							val9.isWarningCleared = true;
							val9.timeToBrake = timeToBrake;
							val9.eolTime = eolTime;
							component4.ServerInsertedInRack(val9);
						}
						if ((UnityEngine.Object)(object)component5 != (UnityEngine.Object)null)
						{
							int timeToBrake2 = ((component5.timeToBrake > 0) ? component5.timeToBrake : 99999);
							int eolTime2 = ((component5.eolTime > 0) ? component5.eolTime : 99999);
							component5.timeToBrake = timeToBrake2;
							component5.eolTime = eolTime2;
							component5.isBroken = false;
							component5.isWarningCleared = true;
							SwitchSaveData val10 = new SwitchSaveData();
							val10.switchID = component5.switchId;
							val10.switchType = component5.switchType;
							val10.rackPositionUID = val4.rackPosGlobalUID;
							val10.position = val7.transform.position;
							val10.rotation = val7.transform.rotation;
							val10.isOn = false;
							val10.label = "";
							val10.isBroken = false;
							val10.isWarningCleared = true;
							val10.timeToBrake = timeToBrake2;
							val10.eolTime = eolTime2;
							component5.SwitchInsertedInRack(val10);
						}
						if ((UnityEngine.Object)(object)component6 != (UnityEngine.Object)null)
						{
							PatchPanelSaveData val11 = new PatchPanelSaveData();
							val11.patchPanelID = component6.patchPanelId;
							val11.patchPanelType = component6.patchPanelType;
							val11.rackPositionUID = val4.rackPosGlobalUID;
							val11.position = val7.transform.position;
							val11.rotation = val7.transform.rotation;
							component6.InsertedInRack(val11);
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
						}
						else if ((UnityEngine.Object)(object)component5 != (UnityEngine.Object)null)
						{
							value = component5.ValidateRackPosition();
						}
						else if ((UnityEngine.Object)(object)component6 != (UnityEngine.Object)null)
						{
							value = component6.ValidateRackPosition();
						}
						((MelonBase)this).LoggerInstance.Msg($"  ValidateRackPosition({itemChoice.name}) => {value}");
					}
					catch (Exception ex2)
					{
						((MelonBase)this).LoggerInstance.Warning("ValidateRackPosition check threw: " + ex2.Message);
					}
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
				((MelonBase)this).LoggerInstance.Error("InstantiateRack returned null — falling back to clone path");
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
			((MelonBase)this).LoggerInstance.Error("InstantiateRack failed: " + ex.Message + " — falling back");
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
		((MelonBase)this).LoggerInstance.Msg($"Rack installed — {removed} cheat items hidden");
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
		if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
		{
			((MelonBase)this).LoggerInstance.Msg($"No UsableObject at anchor {anchorIdx}; skipping");
			return;
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
						val4.cableIDsOnLink = 0;
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
		((MelonBase)this).LoggerInstance.Msg($"Removed {((UnityEngine.Object)((Component)val2).gameObject).name} — anchor U{anchorIdx + 1}, range U{num + 1}–U{num + size} ({size}U)");
	}

	private static string DescribeUsableObject(UsableObject uo)
	{
		Server srv = ((Component)uo).GetComponent<Server>();
		if ((UnityEngine.Object)(object)srv != (UnityEngine.Object)null)
		{
			string s = $"Server {uo.sizeInU}U";
			return srv.isBroken ? (s + " BROKEN") : (!srv.isOn ? (s + " [OFF]") : (s + " [ON]"));
		}
		NetworkSwitch sw = ((Component)uo).GetComponent<NetworkSwitch>();
		if ((UnityEngine.Object)(object)sw != (UnityEngine.Object)null)
			return sw.isBroken ? "Switch BROKEN" : (!sw.isOn ? "Switch [OFF]" : "Switch [ON]");
		PatchPanel pp = ((Component)uo).GetComponent<PatchPanel>();
		if ((UnityEngine.Object)(object)pp != (UnityEngine.Object)null)
			return "Patch Panel";
		return $"{uo.objectInHandType} ({((UnityEngine.Object)(object)((Component)uo).gameObject).name})";
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
				component3.hasInitialized = true;
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
		AddLabel("─────────────────────────────────────────");
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
		((TMP_Text)val12).text = "−";
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
