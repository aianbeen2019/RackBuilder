using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace RackBuilderMod;

public static class NPCBuilder
{
	private static MelonLogger.Instance Log => Melon<RackBuilderCore>.Logger;

	public static void QueueBuildJobs(Rack targetRack, System.Collections.Generic.List<RackBuilderCore.ItemChoice> items, System.Collections.Generic.List<int> slotIndices)
	{
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		TechnicianManager val = UnityEngine.Object.FindObjectOfType<TechnicianManager>();
		MainGameManager val2 = UnityEngine.Object.FindObjectOfType<MainGameManager>();
		if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
		{
			Log.Error("TechnicianManager not found");
			return;
		}
		if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
		{
			Log.Error("MainGameManager not found");
			return;
		}
		Technician val3 = null;
		if (val.technicians != null)
		{
			var enumerator = val.technicians.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Technician current = enumerator.Current;
				if ((UnityEngine.Object)(object)current != (UnityEngine.Object)null && !current.isBusy)
				{
					val3 = current;
					break;
				}
			}
		}
		if ((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null)
		{
			Log.Msg("No available technician — trying to spawn one");
			val3 = SpawnTechnician(val, val2);
		}
		if ((UnityEngine.Object)(object)val3 == (UnityEngine.Object)null)
		{
			Log.Error("Could not find or spawn a technician");
			return;
		}
		Log.Msg($"Using technician: {val3.technicianName} (ID: {val3.technicianID})");
		for (int i = 0; i < items.Count && i < slotIndices.Count; i++)
		{
			RackBuilderCore.ItemChoice itemChoice = items[i];
			int num = slotIndices[i];
			if (num >= ((Il2CppArrayBase<RackPosition>)(object)targetRack.positions).Length)
			{
				continue;
			}
			RackPosition val4 = ((Il2CppArrayBase<RackPosition>)(object)targetRack.positions)[num];
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
					"server" => val2.GetServerPrefab(itemChoice.prefabIndex), 
					"switch" => val2.GetSwitchPrefab(itemChoice.prefabIndex), 
					"patchpanel" => val2.GetPatchPanelPrefab(itemChoice.prefabIndex), 
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
				GameObject val7 = UnityEngine.Object.Instantiate<GameObject>(val6);
				// Reset any cable state inherited from the prefab so newly placed items
				// are never treated as already-cabled before auto-wire is run.
				foreach (CableLink cl in val7.GetComponentsInChildren<CableLink>())
				{
					if ((UnityEngine.Object)(object)cl != (UnityEngine.Object)null)
						cl.cableIDsOnLink = 0;
				}
				val7.transform.SetParent(((Component)val4).transform);
				val7.transform.localPosition = Vector3.zero;
				val7.transform.localRotation = Quaternion.identity;
				Rigidbody component = val7.GetComponent<Rigidbody>();
				if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
				{
					component.isKinematic = true;
					component.useGravity = false;
				}
				Server component2 = val7.GetComponent<Server>();
				if ((UnityEngine.Object)(object)component2 != (UnityEngine.Object)null)
				{
					component2.ServerID = "Build_" + Guid.NewGuid().ToString().Substring(0, 8);
					component2.serverType = itemChoice.prefabIndex;
					((UsableObject)component2).prefabID = itemChoice.prefabIndex;
					component2.isBroken = true;
					component2.isOn = false;
					targetRack.MarkPositionAsUsed(num, itemChoice.sizeInU);
					UsableObject component3 = val7.GetComponent<UsableObject>();
					if ((UnityEngine.Object)(object)component3 != (UnityEngine.Object)null)
					{
						component3.currentRackPosition = val4;
						component3.rackPositionUID = val4.rackPosGlobalUID;
						component3.storedPosition = num;
						component3.sizeInU = itemChoice.sizeInU;
					}
					val.SendTechnician((NetworkSwitch)null, component2);
					Log.Msg($"Sent technician to install {itemChoice.name} at U{num + 1}");
				}
				NetworkSwitch component4 = val7.GetComponent<NetworkSwitch>();
				if ((UnityEngine.Object)(object)component4 != (UnityEngine.Object)null)
				{
					component4.switchId = "Build_" + Guid.NewGuid().ToString().Substring(0, 8);
					component4.switchType = itemChoice.prefabIndex;
					component4.isBroken = true;
					component4.isOn = false;
					targetRack.MarkPositionAsUsed(num, itemChoice.sizeInU);
					UsableObject component5 = val7.GetComponent<UsableObject>();
					if ((UnityEngine.Object)(object)component5 != (UnityEngine.Object)null)
					{
						component5.currentRackPosition = val4;
						component5.rackPositionUID = val4.rackPosGlobalUID;
						component5.storedPosition = num;
						component5.sizeInU = itemChoice.sizeInU;
					}
					val.SendTechnician(component4, (Server)null);
					Log.Msg($"Sent technician to install {itemChoice.name} at U{num + 1}");
				}
			}
			catch (Exception ex)
			{
				Log.Error("Build job failed for " + itemChoice.name + ": " + ex.Message);
			}
		}
	}

	private static Technician SpawnTechnician(TechnicianManager techMgr, MainGameManager mgr)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (mgr.techniciansPrefabs == null || ((Il2CppArrayBase<GameObject>)(object)mgr.techniciansPrefabs).Length == 0)
		{
			Log.Error("No technician prefabs available");
			return null;
		}
		try
		{
			GameObject val = ((Il2CppArrayBase<GameObject>)(object)mgr.techniciansPrefabs)[0];
			GameObject val2 = UnityEngine.Object.Instantiate<GameObject>(val);
			Technician component = val2.GetComponent<Technician>();
			if ((UnityEngine.Object)(object)component == (UnityEngine.Object)null)
			{
				Log.Error("Spawned technician has no Technician component");
				return null;
			}
			component.technicianID = ((techMgr.technicians != null) ? techMgr.technicians.Count : 0);
			component.technicianName = "Builder";
			if (techMgr.transformIdle != null && ((Il2CppArrayBase<Transform>)(object)techMgr.transformIdle).Length > 0)
			{
				component.transformIdle = ((Il2CppArrayBase<Transform>)(object)techMgr.transformIdle)[0];
			}
			if (techMgr.transformContainer != null && ((Il2CppArrayBase<Transform>)(object)techMgr.transformContainer).Length > 0)
				component.transformContainer = ((Il2CppArrayBase<Transform>)(object)techMgr.transformContainer)[0];
			if (techMgr.transformDumpster != null && ((Il2CppArrayBase<Transform>)(object)techMgr.transformDumpster).Length > 0)
				component.transformDumpster = ((Il2CppArrayBase<Transform>)(object)techMgr.transformDumpster)[0];
			if (techMgr.transformDeviceSpawnPosition != null && ((Il2CppArrayBase<Transform>)(object)techMgr.transformDeviceSpawnPosition).Length > 0)
				component.transformDeviceSpawnPosition = ((Il2CppArrayBase<Transform>)(object)techMgr.transformDeviceSpawnPosition)[0];
			if ((UnityEngine.Object)(object)component.transformIdle != (UnityEngine.Object)null)
			{
				val2.transform.position = component.transformIdle.position;
			}
			techMgr.AddTechnician(component);
			Log.Msg($"Spawned technician: {component.technicianName} (ID: {component.technicianID})");
			return component;
		}
		catch (Exception ex)
		{
			Log.Error("Spawn technician failed: " + ex.Message);
			return null;
		}
	}
}
