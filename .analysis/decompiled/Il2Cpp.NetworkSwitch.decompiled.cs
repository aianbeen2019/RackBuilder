using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Text;
using Il2CppTMPro;
using UnityEngine;

namespace Il2Cpp;

public class NetworkSwitch : UsableObject
{
	[ObfuscatedName("NetworkSwitch+<>c__DisplayClass27_0")]
	public sealed class __c__DisplayClass27_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___4__this;

		private static readonly System.IntPtr NativeFieldInfoPtr_candidate;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__GenerateUniqueSwitchId_b__0_Internal_Boolean_NetworkSwitch_0;

		public unsafe NetworkSwitch __4__this
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___4__this);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<NetworkSwitch>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___4__this)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe string candidate
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_candidate);
				return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_candidate)), IL2CPP.ManagedStringToIl2Cpp(value));
			}
		}

		static __c__DisplayClass27_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass27_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "<>c__DisplayClass27_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass27_0>.NativeClassPtr);
			NativeFieldInfoPtr___4__this = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass27_0>.NativeClassPtr, "<>4__this");
			NativeFieldInfoPtr_candidate = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass27_0>.NativeClassPtr, "candidate");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass27_0>.NativeClassPtr, 100664405);
			NativeMethodInfoPtr__GenerateUniqueSwitchId_b__0_Internal_Boolean_NetworkSwitch_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass27_0>.NativeClassPtr, 100664406);
		}

		[CallerCount(2371)]
		[CachedScanResults(RefRangeStart = 316, RefRangeEnd = 2687, XrefRangeStart = 316, XrefRangeEnd = 2687, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass27_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass27_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38704, XrefRangeEnd = 38708, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _GenerateUniqueSwitchId_b__0(NetworkSwitch s)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)s);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__GenerateUniqueSwitchId_b__0_Internal_Boolean_NetworkSwitch_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass27_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr_canvas;

	private static readonly System.IntPtr NativeFieldInfoPtr_txtScreen;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableLinkSwitchPorts;

	private static readonly System.IntPtr NativeFieldInfoPtr_switchId;

	private static readonly System.IntPtr NativeFieldInfoPtr_switchType;

	private static readonly System.IntPtr NativeFieldInfoPtr_isOn;

	private static readonly System.IntPtr NativeFieldInfoPtr_timeToBrake;

	private static readonly System.IntPtr NativeFieldInfoPtr_eolTime;

	private static readonly System.IntPtr NativeFieldInfoPtr_isBroken;

	private static readonly System.IntPtr NativeFieldInfoPtr_existingWarningSigns;

	private static readonly System.IntPtr NativeFieldInfoPtr_existingErrorSigns;

	private static readonly System.IntPtr NativeFieldInfoPtr_isWarningCleared;

	private static readonly System.IntPtr NativeFieldInfoPtr_powerButton;

	private static readonly System.IntPtr NativeFieldInfoPtr_temporarilyDisconnectedCables;

	private static readonly System.IntPtr NativeFieldInfoPtr_disallowedVlansPerPort;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastDisplayedEolMinute;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastDisplayedLabel;

	private static readonly System.IntPtr NativeFieldInfoPtr_sb;

	private static readonly System.IntPtr NativeMethodInfoPtr_Awake_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Start_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_PowerButton_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TurnOffCommonFunctions_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TurnOnCommonFunction_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsAnyCableConnected_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InteractOnClick_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InteractOnHover_Public_Virtual_Void_RaycastHit_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SwitchInsertedInRack_Public_Void_SwitchSaveData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GenerateUniqueSwitchId_Private_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_DisconnectCablesWhenSwitchIsOff_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HandleNewCableWhileOff_Public_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetConnectedDevices_Public_List_1_ValueTuple_2_String_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetSwitchId_Public_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TickTimer_Public_Virtual_Final_New_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UpdateScreenUI_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AppendEolTime_Private_Static_Void_StringBuilder_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ItIsBroken_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_DisconnectCables_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ReconnectCables_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_PatchStaleSwitchId_Private_Void_byref_CableEndpoint_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValidateRackPosition_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ButtonShowNetworkSwitchConfig_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ClearWarningSign_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ClearErrorSign_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnDestroy_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetPowerLightMaterial_Private_Void_Material_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RepairDevice_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsVlanAllowedOnPort_Public_Boolean_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsVlanAllowedOnCable_Public_Boolean_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetVlanDisallowed_Public_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetVlanAllowed_Public_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetDisallowedVlans_Public_HashSet_1_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetAllDisallowedVlans_Public_Dictionary_2_Int32_HashSet_1_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetDisallowedVlansPerPort_Public_Void_Dictionary_2_Int32_HashSet_1_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__SwitchInsertedInRack_b__26_0_Private_Boolean_NetworkSwitch_0;

	public unsafe GameObject canvas
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_canvas);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameObject>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_canvas)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TextMeshProUGUI txtScreen
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_txtScreen);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TextMeshProUGUI>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_txtScreen)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Il2CppReferenceArray<CableLink> cableLinkSwitchPorts
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableLinkSwitchPorts);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppReferenceArray<CableLink>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableLinkSwitchPorts)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe string switchId
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_switchId);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_switchId)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe int switchType
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_switchType);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_switchType)) = value;
		}
	}

	public unsafe bool isOn
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isOn);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isOn)) = value;
		}
	}

	public unsafe int timeToBrake
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_timeToBrake);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_timeToBrake)) = value;
		}
	}

	public unsafe int eolTime
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_eolTime);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_eolTime)) = value;
		}
	}

	public unsafe bool isBroken
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isBroken);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isBroken)) = value;
		}
	}

	public unsafe int existingWarningSigns
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_existingWarningSigns);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_existingWarningSigns)) = value;
		}
	}

	public unsafe int existingErrorSigns
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_existingErrorSigns);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_existingErrorSigns)) = value;
		}
	}

	public unsafe bool isWarningCleared
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isWarningCleared);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isWarningCleared)) = value;
		}
	}

	public unsafe Renderer powerButton
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_powerButton);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Renderer>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_powerButton)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe HashSet<int> temporarilyDisconnectedCables
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_temporarilyDisconnectedCables);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<HashSet<int>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_temporarilyDisconnectedCables)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, HashSet<int>> disallowedVlansPerPort
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_disallowedVlansPerPort);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, HashSet<int>>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_disallowedVlansPerPort)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int lastDisplayedEolMinute
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastDisplayedEolMinute);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastDisplayedEolMinute)) = value;
		}
	}

	public unsafe string lastDisplayedLabel
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastDisplayedLabel);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastDisplayedLabel)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe static StringBuilder sb
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_sb, (void*)(&intPtr));
			System.IntPtr intPtr2 = intPtr;
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<StringBuilder>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_sb, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	static NetworkSwitch()
	{
		Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "NetworkSwitch");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr);
		NativeFieldInfoPtr_canvas = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "canvas");
		NativeFieldInfoPtr_txtScreen = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "txtScreen");
		NativeFieldInfoPtr_cableLinkSwitchPorts = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "cableLinkSwitchPorts");
		NativeFieldInfoPtr_switchId = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "switchId");
		NativeFieldInfoPtr_switchType = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "switchType");
		NativeFieldInfoPtr_isOn = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "isOn");
		NativeFieldInfoPtr_timeToBrake = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "timeToBrake");
		NativeFieldInfoPtr_eolTime = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "eolTime");
		NativeFieldInfoPtr_isBroken = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "isBroken");
		NativeFieldInfoPtr_existingWarningSigns = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "existingWarningSigns");
		NativeFieldInfoPtr_existingErrorSigns = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "existingErrorSigns");
		NativeFieldInfoPtr_isWarningCleared = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "isWarningCleared");
		NativeFieldInfoPtr_powerButton = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "powerButton");
		NativeFieldInfoPtr_temporarilyDisconnectedCables = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "temporarilyDisconnectedCables");
		NativeFieldInfoPtr_disallowedVlansPerPort = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "disallowedVlansPerPort");
		NativeFieldInfoPtr_lastDisplayedEolMinute = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "lastDisplayedEolMinute");
		NativeFieldInfoPtr_lastDisplayedLabel = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "lastDisplayedLabel");
		NativeFieldInfoPtr_sb = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, "sb");
		NativeMethodInfoPtr_Awake_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664367);
		NativeMethodInfoPtr_Start_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664368);
		NativeMethodInfoPtr_PowerButton_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664369);
		NativeMethodInfoPtr_TurnOffCommonFunctions_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664370);
		NativeMethodInfoPtr_TurnOnCommonFunction_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664371);
		NativeMethodInfoPtr_IsAnyCableConnected_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664372);
		NativeMethodInfoPtr_InteractOnClick_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664373);
		NativeMethodInfoPtr_InteractOnHover_Public_Virtual_Void_RaycastHit_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664374);
		NativeMethodInfoPtr_SwitchInsertedInRack_Public_Void_SwitchSaveData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664375);
		NativeMethodInfoPtr_GenerateUniqueSwitchId_Private_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664376);
		NativeMethodInfoPtr_DisconnectCablesWhenSwitchIsOff_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664377);
		NativeMethodInfoPtr_HandleNewCableWhileOff_Public_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664378);
		NativeMethodInfoPtr_GetConnectedDevices_Public_List_1_ValueTuple_2_String_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664379);
		NativeMethodInfoPtr_GetSwitchId_Public_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664380);
		NativeMethodInfoPtr_TickTimer_Public_Virtual_Final_New_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664381);
		NativeMethodInfoPtr_UpdateScreenUI_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664382);
		NativeMethodInfoPtr_AppendEolTime_Private_Static_Void_StringBuilder_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664383);
		NativeMethodInfoPtr_ItIsBroken_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664384);
		NativeMethodInfoPtr_DisconnectCables_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664385);
		NativeMethodInfoPtr_ReconnectCables_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664386);
		NativeMethodInfoPtr_PatchStaleSwitchId_Private_Void_byref_CableEndpoint_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664387);
		NativeMethodInfoPtr_ValidateRackPosition_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664388);
		NativeMethodInfoPtr_ButtonShowNetworkSwitchConfig_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664389);
		NativeMethodInfoPtr_ClearWarningSign_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664390);
		NativeMethodInfoPtr_ClearErrorSign_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664391);
		NativeMethodInfoPtr_OnDestroy_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664392);
		NativeMethodInfoPtr_SetPowerLightMaterial_Private_Void_Material_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664393);
		NativeMethodInfoPtr_RepairDevice_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664394);
		NativeMethodInfoPtr_IsVlanAllowedOnPort_Public_Boolean_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664395);
		NativeMethodInfoPtr_IsVlanAllowedOnCable_Public_Boolean_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664396);
		NativeMethodInfoPtr_SetVlanDisallowed_Public_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664397);
		NativeMethodInfoPtr_SetVlanAllowed_Public_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664398);
		NativeMethodInfoPtr_GetDisallowedVlans_Public_HashSet_1_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664399);
		NativeMethodInfoPtr_GetAllDisallowedVlans_Public_Dictionary_2_Int32_HashSet_1_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664400);
		NativeMethodInfoPtr_SetDisallowedVlansPerPort_Public_Void_Dictionary_2_Int32_HashSet_1_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664401);
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664402);
		NativeMethodInfoPtr__SwitchInsertedInRack_b__26_0_Private_Boolean_NetworkSwitch_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr, 100664404);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38708, XrefRangeEnd = 38716, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void Awake()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Awake_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38716, XrefRangeEnd = 38725, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Start()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Start_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 38735, RefRangeEnd = 38736, XrefRangeStart = 38725, XrefRangeEnd = 38735, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void PowerButton(bool forceState = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&forceState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_PowerButton_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 38751, RefRangeEnd = 38753, XrefRangeStart = 38736, XrefRangeEnd = 38751, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void TurnOffCommonFunctions()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TurnOffCommonFunctions_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 38773, RefRangeEnd = 38776, XrefRangeStart = 38753, XrefRangeEnd = 38773, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void TurnOnCommonFunction()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TurnOnCommonFunction_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 38781, RefRangeEnd = 38783, XrefRangeStart = 38776, XrefRangeEnd = 38781, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsAnyCableConnected()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsAnyCableConnected_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38783, XrefRangeEnd = 38815, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void InteractOnClick()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_InteractOnClick_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38815, XrefRangeEnd = 38817, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void InteractOnHover(RaycastHit hit)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&hit);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_InteractOnHover_Public_Virtual_Void_RaycastHit_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 38925, RefRangeEnd = 38927, XrefRangeStart = 38817, XrefRangeEnd = 38925, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SwitchInsertedInRack(SwitchSaveData switchSaveData = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)switchSaveData);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SwitchInsertedInRack_Public_Void_SwitchSaveData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 38981, RefRangeEnd = 38984, XrefRangeStart = 38927, XrefRangeEnd = 38981, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe string GenerateUniqueSwitchId()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GenerateUniqueSwitchId_Private_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return IL2CPP.Il2CppStringToManaged(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38984, XrefRangeEnd = 38985, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void DisconnectCablesWhenSwitchIsOff()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_DisconnectCablesWhenSwitchIsOff_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38985, XrefRangeEnd = 38999, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void HandleNewCableWhileOff(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_HandleNewCableWhileOff_Public_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 38999, XrefRangeEnd = 39021, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<Il2CppSystem.ValueTuple<string, int>> GetConnectedDevices()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetConnectedDevices_Public_List_1_ValueTuple_2_String_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Il2CppSystem.ValueTuple<string, int>>>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe string GetSwitchId()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetSwitchId_Public_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return IL2CPP.Il2CppStringToManaged(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39021, XrefRangeEnd = 39054, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual void TickTimer()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TickTimer_Public_Virtual_Final_New_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39054, XrefRangeEnd = 39073, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void UpdateScreenUI()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_UpdateScreenUI_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 39088, RefRangeEnd = 39090, XrefRangeStart = 39073, XrefRangeEnd = 39088, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void AppendEolTime(StringBuilder builder, int eolSeconds)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)builder);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &eolSeconds;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AppendEolTime_Private_Static_Void_StringBuilder_Int32_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 39117, RefRangeEnd = 39118, XrefRangeStart = 39090, XrefRangeEnd = 39117, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ItIsBroken()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ItIsBroken_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 39133, RefRangeEnd = 39135, XrefRangeStart = 39118, XrefRangeEnd = 39133, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void DisconnectCables()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_DisconnectCables_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 39172, RefRangeEnd = 39173, XrefRangeStart = 39135, XrefRangeEnd = 39172, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ReconnectCables()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ReconnectCables_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 39190, RefRangeEnd = 39192, XrefRangeStart = 39173, XrefRangeEnd = 39190, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void PatchStaleSwitchId(ref WaypointInitializationSystem.CableEndpoint endpoint)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)endpoint));
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_PatchStaleSwitchId_Private_Void_byref_CableEndpoint_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39192, XrefRangeEnd = 39204, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool ValidateRackPosition()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValidateRackPosition_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39204, XrefRangeEnd = 39208, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ButtonShowNetworkSwitchConfig()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ButtonShowNetworkSwitchConfig_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 39211, RefRangeEnd = 39215, XrefRangeStart = 39208, XrefRangeEnd = 39211, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ClearWarningSign(bool isPreserved = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&isPreserved);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ClearWarningSign_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39215, XrefRangeEnd = 39218, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ClearErrorSign()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ClearErrorSign_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39218, XrefRangeEnd = 39234, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnDestroy()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnDestroy_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 39243, RefRangeEnd = 39246, XrefRangeStart = 39234, XrefRangeEnd = 39243, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetPowerLightMaterial(Material material)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)material);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetPowerLightMaterial_Private_Void_Material_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 39270, RefRangeEnd = 39271, XrefRangeStart = 39246, XrefRangeEnd = 39270, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RepairDevice()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RepairDevice_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 39277, RefRangeEnd = 39278, XrefRangeStart = 39271, XrefRangeEnd = 39277, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsVlanAllowedOnPort(int portIndex, int vlanId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&portIndex);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &vlanId;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsVlanAllowedOnPort_Public_Boolean_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 39280, RefRangeEnd = 39282, XrefRangeStart = 39278, XrefRangeEnd = 39280, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsVlanAllowedOnCable(int cableId, int vlanId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&cableId);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &vlanId;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsVlanAllowedOnCable_Public_Boolean_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 39297, RefRangeEnd = 39299, XrefRangeStart = 39282, XrefRangeEnd = 39297, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetVlanDisallowed(int portIndex, int vlanId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&portIndex);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &vlanId;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetVlanDisallowed_Public_Void_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 39309, RefRangeEnd = 39311, XrefRangeStart = 39299, XrefRangeEnd = 39309, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetVlanAllowed(int portIndex, int vlanId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&portIndex);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &vlanId;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetVlanAllowed_Public_Void_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 39316, RefRangeEnd = 39320, XrefRangeStart = 39311, XrefRangeEnd = 39316, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe HashSet<int> GetDisallowedVlans(int portIndex)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&portIndex);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetDisallowedVlans_Public_HashSet_1_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<HashSet<int>>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe Dictionary<int, HashSet<int>> GetAllDisallowedVlans()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetAllDisallowedVlans_Public_Dictionary_2_Int32_HashSet_1_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, HashSet<int>>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39320, XrefRangeEnd = 39328, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetDisallowedVlansPerPort(Dictionary<int, HashSet<int>> data)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)data);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetDisallowedVlansPerPort_Public_Void_Dictionary_2_Int32_HashSet_1_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39328, XrefRangeEnd = 39343, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe NetworkSwitch()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<NetworkSwitch>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 39343, XrefRangeEnd = 39347, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool _SwitchInsertedInRack_b__26_0(NetworkSwitch s)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)s);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__SwitchInsertedInRack_b__26_0_Private_Boolean_NetworkSwitch_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public NetworkSwitch(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
