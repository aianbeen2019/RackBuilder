using System;
using System.Runtime.CompilerServices;
using Il2CppEPOOutline;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Il2Cpp;

public class CableLink : Interact
{
	[OriginalName("Assembly-CSharp.dll", "", "TypeOfLink")]
	public enum TypeOfLink
	{
		None,
		Server,
		Switch,
		Base,
		LB,
		PatchPanel
	}

	private static readonly IntPtr NativeFieldInfoPtr_outlineEffect;

	private static readonly IntPtr NativeFieldInfoPtr_isStartOrEnd;

	private static readonly IntPtr NativeFieldInfoPtr_isEndPoint;

	private static readonly IntPtr NativeFieldInfoPtr_cableIDsOnLink;

	private static readonly IntPtr NativeFieldInfoPtr_switchID;

	private static readonly IntPtr NativeFieldInfoPtr_typeOfLink;

	private static readonly IntPtr NativeFieldInfoPtr_connectionSpeed;

	private static readonly IntPtr NativeFieldInfoPtr_CustomerID;

	private static readonly IntPtr NativeFieldInfoPtr_parentServer;

	private static readonly IntPtr NativeFieldInfoPtr_parentSwitch;

	private static readonly IntPtr NativeFieldInfoPtr_parentPatchPanel;

	private static readonly IntPtr NativeFieldInfoPtr_isSFPPort;

	private static readonly IntPtr NativeFieldInfoPtr_sfpTypeInserted;

	private static readonly IntPtr NativeFieldInfoPtr_sfpTypeSupported;

	private static readonly IntPtr NativeFieldInfoPtr_isFibrePort;

	private static readonly IntPtr NativeFieldInfoPtr_insertedSFP;

	private static readonly IntPtr NativeFieldInfoPtr_ropeForwardOffset;

	private static readonly IntPtr NativeFieldInfoPtr_sfpForwardOffset;

	private static readonly IntPtr NativeFieldInfoPtr_ropeAttachPoint;

	private static readonly IntPtr NativeMethodInfoPtr_Start_Private_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_SetConnectionSpeed_Public_Void_Single_0;

	private static readonly IntPtr NativeMethodInfoPtr_InsertSFP_Public_Void_Single_Int32_SFPModule_0;

	private static readonly IntPtr NativeMethodInfoPtr_RemoveSFP_Public_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_InteractOnClick_Public_Virtual_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_IsAllowedToDoSecondAction_Public_Virtual_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_SecondActionOnClick_Public_Virtual_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_CollectPatchPanelChainCables_Private_List_1_Int32_Int32_0;

	private static readonly IntPtr NativeMethodInfoPtr_LabelActionOnClick_Public_Virtual_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_InteractOnHover_Public_Virtual_Void_RaycastHit_0;

	private static readonly IntPtr NativeMethodInfoPtr_OnHoverOver_Public_Virtual_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_CreateRopeAttachPoint_Private_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_GetRopeAttachPoint_Public_Transform_0;

	private static readonly IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	public unsafe Outlinable outlineEffect
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_outlineEffect);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<Outlinable>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_outlineEffect)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe bool isStartOrEnd
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isStartOrEnd);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isStartOrEnd)) = value;
		}
	}

	public unsafe bool isEndPoint
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isEndPoint);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isEndPoint)) = value;
		}
	}

	public unsafe int cableIDsOnLink
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableIDsOnLink);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableIDsOnLink)) = value;
		}
	}

	public unsafe string switchID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_switchID);
			return IL2CPP.Il2CppStringToManaged(*(IntPtr*)num);
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_switchID)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe TypeOfLink typeOfLink
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_typeOfLink);
			return *(TypeOfLink*)num;
		}
		set
		{
			*(TypeOfLink*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_typeOfLink)) = value;
		}
	}

	public unsafe float connectionSpeed
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_connectionSpeed);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_connectionSpeed)) = value;
		}
	}

	public unsafe int CustomerID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_CustomerID);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_CustomerID)) = value;
		}
	}

	public unsafe Server parentServer
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentServer);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<Server>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentServer)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe NetworkSwitch parentSwitch
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentSwitch);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<NetworkSwitch>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentSwitch)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe PatchPanel parentPatchPanel
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentPatchPanel);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<PatchPanel>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentPatchPanel)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe bool isSFPPort
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isSFPPort);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isSFPPort)) = value;
		}
	}

	public unsafe int sfpTypeInserted
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_sfpTypeInserted);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_sfpTypeInserted)) = value;
		}
	}

	public unsafe int sfpTypeSupported
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_sfpTypeSupported);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_sfpTypeSupported)) = value;
		}
	}

	public unsafe bool isFibrePort
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isFibrePort);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isFibrePort)) = value;
		}
	}

	public unsafe SFPModule insertedSFP
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_insertedSFP);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<SFPModule>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_insertedSFP)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe float ropeForwardOffset
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ropeForwardOffset);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ropeForwardOffset)) = value;
		}
	}

	public unsafe float sfpForwardOffset
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_sfpForwardOffset);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_sfpForwardOffset)) = value;
		}
	}

	public unsafe Transform ropeAttachPoint
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ropeAttachPoint);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<Transform>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ropeAttachPoint)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	static CableLink()
	{
		Il2CppClassPointerStore<CableLink>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "CableLink");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<CableLink>.NativeClassPtr);
		NativeFieldInfoPtr_outlineEffect = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "outlineEffect");
		NativeFieldInfoPtr_isStartOrEnd = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "isStartOrEnd");
		NativeFieldInfoPtr_isEndPoint = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "isEndPoint");
		NativeFieldInfoPtr_cableIDsOnLink = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "cableIDsOnLink");
		NativeFieldInfoPtr_switchID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "switchID");
		NativeFieldInfoPtr_typeOfLink = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "typeOfLink");
		NativeFieldInfoPtr_connectionSpeed = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "connectionSpeed");
		NativeFieldInfoPtr_CustomerID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "CustomerID");
		NativeFieldInfoPtr_parentServer = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "parentServer");
		NativeFieldInfoPtr_parentSwitch = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "parentSwitch");
		NativeFieldInfoPtr_parentPatchPanel = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "parentPatchPanel");
		NativeFieldInfoPtr_isSFPPort = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "isSFPPort");
		NativeFieldInfoPtr_sfpTypeInserted = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "sfpTypeInserted");
		NativeFieldInfoPtr_sfpTypeSupported = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "sfpTypeSupported");
		NativeFieldInfoPtr_isFibrePort = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "isFibrePort");
		NativeFieldInfoPtr_insertedSFP = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "insertedSFP");
		NativeFieldInfoPtr_ropeForwardOffset = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "ropeForwardOffset");
		NativeFieldInfoPtr_sfpForwardOffset = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "sfpForwardOffset");
		NativeFieldInfoPtr_ropeAttachPoint = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableLink>.NativeClassPtr, "ropeAttachPoint");
		NativeMethodInfoPtr_Start_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664196);
		NativeMethodInfoPtr_SetConnectionSpeed_Public_Void_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664197);
		NativeMethodInfoPtr_InsertSFP_Public_Void_Single_Int32_SFPModule_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664198);
		NativeMethodInfoPtr_RemoveSFP_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664199);
		NativeMethodInfoPtr_InteractOnClick_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664200);
		NativeMethodInfoPtr_IsAllowedToDoSecondAction_Public_Virtual_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664201);
		NativeMethodInfoPtr_SecondActionOnClick_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664202);
		NativeMethodInfoPtr_CollectPatchPanelChainCables_Private_List_1_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664203);
		NativeMethodInfoPtr_LabelActionOnClick_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664204);
		NativeMethodInfoPtr_InteractOnHover_Public_Virtual_Void_RaycastHit_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664205);
		NativeMethodInfoPtr_OnHoverOver_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664206);
		NativeMethodInfoPtr_CreateRopeAttachPoint_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664207);
		NativeMethodInfoPtr_GetRopeAttachPoint_Public_Transform_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664208);
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableLink>.NativeClassPtr, 100664209);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36579, XrefRangeEnd = 36597, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Start()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Start_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe void SetConnectionSpeed(float speed)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = (nint)(&speed);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetConnectionSpeed_Public_Void_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 36598, RefRangeEnd = 36600, XrefRangeStart = 36597, XrefRangeEnd = 36598, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void InsertSFP(float speed, int type, SFPModule module)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[3];
		*ptr = (nint)(&speed);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = &type;
		*(IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)module);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InsertSFP_Public_Void_Single_Int32_SFPModule_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 36601, RefRangeEnd = 36603, XrefRangeStart = 36600, XrefRangeEnd = 36601, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveSFP()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveSFP_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36603, XrefRangeEnd = 36637, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void InteractOnClick()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_InteractOnClick_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36637, XrefRangeEnd = 36642, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool IsAllowedToDoSecondAction()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IsAllowedToDoSecondAction_Public_Virtual_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36642, XrefRangeEnd = 36708, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void SecondActionOnClick()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_SecondActionOnClick_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36708, XrefRangeEnd = 36745, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<int> CollectPatchPanelChainCables(int startCableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = (nint)(&startCableId);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CollectPatchPanelChainCables_Private_List_1_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<List<int>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36745, XrefRangeEnd = 36762, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void LabelActionOnClick()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_LabelActionOnClick_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36762, XrefRangeEnd = 36799, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void InteractOnHover(RaycastHit hit)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = (nint)(&hit);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_InteractOnHover_Public_Virtual_Void_RaycastHit_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36799, XrefRangeEnd = 36806, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnHoverOver()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnHoverOver_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36806, XrefRangeEnd = 36820, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void CreateRopeAttachPoint()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CreateRopeAttachPoint_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 36841, RefRangeEnd = 36842, XrefRangeStart = 36820, XrefRangeEnd = 36841, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Transform GetRopeAttachPoint()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetRopeAttachPoint_Public_Transform_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<Transform>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 36842, XrefRangeEnd = 36847, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe CableLink()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<CableLink>.NativeClassPtr))
	{
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public CableLink(IntPtr pointer)
		: base(pointer)
	{
	}
}
