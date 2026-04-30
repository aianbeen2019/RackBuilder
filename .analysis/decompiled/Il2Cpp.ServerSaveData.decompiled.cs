using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using UnityEngine;

namespace Il2Cpp;

[System.Serializable]
public class ServerSaveData : Il2CppSystem.Object
{
	private static readonly System.IntPtr NativeFieldInfoPtr_serverID;

	private static readonly System.IntPtr NativeFieldInfoPtr_customerID;

	private static readonly System.IntPtr NativeFieldInfoPtr_ip;

	private static readonly System.IntPtr NativeFieldInfoPtr_serverType;

	private static readonly System.IntPtr NativeFieldInfoPtr_position;

	private static readonly System.IntPtr NativeFieldInfoPtr_rotation;

	private static readonly System.IntPtr NativeFieldInfoPtr_rackPositionUID;

	private static readonly System.IntPtr NativeFieldInfoPtr_prefabID;

	private static readonly System.IntPtr NativeFieldInfoPtr_isOn;

	private static readonly System.IntPtr NativeFieldInfoPtr_isBroken;

	private static readonly System.IntPtr NativeFieldInfoPtr_timeToBrake;

	private static readonly System.IntPtr NativeFieldInfoPtr_eolTime;

	private static readonly System.IntPtr NativeFieldInfoPtr_isWarningCleared;

	private static readonly System.IntPtr NativeFieldInfoPtr_label;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	public unsafe string serverID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_serverID);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_serverID)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe int customerID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_customerID);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_customerID)) = value;
		}
	}

	public unsafe string ip
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ip);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ip)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe int serverType
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_serverType);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_serverType)) = value;
		}
	}

	public unsafe Vector3 position
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_position);
			return *(Vector3*)num;
		}
		set
		{
			*(Vector3*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_position)) = value;
		}
	}

	public unsafe Quaternion rotation
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rotation);
			return *(Quaternion*)num;
		}
		set
		{
			*(Quaternion*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rotation)) = value;
		}
	}

	public unsafe int rackPositionUID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rackPositionUID);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rackPositionUID)) = value;
		}
	}

	public unsafe int prefabID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_prefabID);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_prefabID)) = value;
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

	public unsafe string label
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_label);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_label)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	static ServerSaveData()
	{
		Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "ServerSaveData");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr);
		NativeFieldInfoPtr_serverID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "serverID");
		NativeFieldInfoPtr_customerID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "customerID");
		NativeFieldInfoPtr_ip = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "ip");
		NativeFieldInfoPtr_serverType = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "serverType");
		NativeFieldInfoPtr_position = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "position");
		NativeFieldInfoPtr_rotation = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "rotation");
		NativeFieldInfoPtr_rackPositionUID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "rackPositionUID");
		NativeFieldInfoPtr_prefabID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "prefabID");
		NativeFieldInfoPtr_isOn = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "isOn");
		NativeFieldInfoPtr_isBroken = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "isBroken");
		NativeFieldInfoPtr_timeToBrake = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "timeToBrake");
		NativeFieldInfoPtr_eolTime = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "eolTime");
		NativeFieldInfoPtr_isWarningCleared = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "isWarningCleared");
		NativeFieldInfoPtr_label = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, "label");
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr, 100663783);
	}

	[CallerCount(2371)]
	[CachedScanResults(RefRangeStart = 316, RefRangeEnd = 2687, XrefRangeStart = 316, XrefRangeEnd = 2687, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe ServerSaveData()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<ServerSaveData>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public ServerSaveData(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
