using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Il2Cpp;

public class CablePositions : MonoBehaviour
{
	public class CableEntityInfo : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_Entity;

		private static readonly System.IntPtr NativeFieldInfoPtr_Version;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		public unsafe Entity Entity
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_Entity);
				return *(Entity*)num;
			}
			set
			{
				*(Entity*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_Entity)) = value;
			}
		}

		public unsafe int Version
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_Version);
				return *(int*)num;
			}
			set
			{
				*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_Version)) = value;
			}
		}

		static CableEntityInfo()
		{
			Il2CppClassPointerStore<CableEntityInfo>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "CableEntityInfo");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<CableEntityInfo>.NativeClassPtr);
			NativeFieldInfoPtr_Entity = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableEntityInfo>.NativeClassPtr, "Entity");
			NativeFieldInfoPtr_Version = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CableEntityInfo>.NativeClassPtr, "Version");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CableEntityInfo>.NativeClassPtr, 100663423);
		}

		[CallerCount(2371)]
		[CachedScanResults(RefRangeStart = 316, RefRangeEnd = 2687, XrefRangeStart = 316, XrefRangeEnd = 2687, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe CableEntityInfo()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<CableEntityInfo>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public CableEntityInfo(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr_instance;

	private static readonly System.IntPtr NativeFieldInfoPtr_cables;

	private static readonly System.IntPtr NativeFieldInfoPtr_rawCablePoints;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableEntities;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableGameObjects;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableMaterials;

	private static readonly System.IntPtr NativeFieldInfoPtr_nextCableId;

	private static readonly System.IntPtr NativeFieldInfoPtr_packetSpeed;

	private static readonly System.IntPtr NativeFieldInfoPtr_spawnRateDivider;

	private static readonly System.IntPtr NativeFieldInfoPtr_startSwitchID;

	private static readonly System.IntPtr NativeFieldInfoPtr_endSwitchID;

	private static readonly System.IntPtr NativeFieldInfoPtr_startCustomerID;

	private static readonly System.IntPtr NativeFieldInfoPtr_endCustomerID;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableStartPoints;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableEndPoints;

	private static readonly System.IntPtr NativeFieldInfoPtr_rawLinkTransforms;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableWidth;

	private static readonly System.IntPtr NativeFieldInfoPtr_cableMaterial;

	private static readonly System.IntPtr NativeFieldInfoPtr_bendRadius;

	private static readonly System.IntPtr NativeFieldInfoPtr_bendSegments;

	private static readonly System.IntPtr NativeFieldInfoPtr_holderSegmentLength;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastPosAfterBendToPass;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastCompletedCableId;

	private static readonly System.IntPtr NativeFieldInfoPtr_startCableLinkType;

	private static readonly System.IntPtr NativeFieldInfoPtr_endtCableLinkType;

	private static readonly System.IntPtr NativeFieldInfoPtr_startServerID;

	private static readonly System.IntPtr NativeFieldInfoPtr_endServerID;

	private static readonly System.IntPtr NativeFieldInfoPtr_currentCableLength;

	private static readonly System.IntPtr NativeFieldInfoPtr_totalCableLengthLaid;

	private static readonly System.IntPtr NativeMethodInfoPtr_Awake_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Start_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ClearAllCables_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_LoadCable_Public_Void_CableSaveData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CreateNewCable_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CreateNewReverseCable_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AssignNewPosition_Public_Void_Int32_Transform_Boolean_Boolean_TypeOfLink_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GenerateFinalPath_Private_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GenerateCornerBend_Private_IEnumerable_1_Vector3_Vector3_Vector3_Vector3_Transform_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GenerateBentSegment_Private_IEnumerable_1_Vector3_Vector3_Vector3_Transform_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RedrawCable_Private_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CreateTubeMesh_Private_Mesh_List_1_Vector3_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemovePosition_Public_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveLastPosition_Public_Transform_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetCablePositions_Public_List_1_Vector3_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetRawCablePositions_Public_List_1_Vector3_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetRawLinkTransforms_Public_List_1_Transform_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AssignEntity_Public_Void_Int32_Entity_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsCableComplete_Public_Boolean_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetCableMaterial_Public_Material_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	public unsafe static CablePositions instance
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_instance, (void*)(&intPtr));
			System.IntPtr intPtr2 = intPtr;
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<CablePositions>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_instance, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, List<Vector3>> cables
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cables);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, List<Vector3>>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cables)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, List<Vector3>> rawCablePoints
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rawCablePoints);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, List<Vector3>>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rawCablePoints)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, CableEntityInfo> cableEntities
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableEntities);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, CableEntityInfo>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableEntities)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, GameObject> cableGameObjects
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableGameObjects);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, GameObject>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableGameObjects)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, Material> cableMaterials
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableMaterials);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, Material>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableMaterials)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int nextCableId
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_nextCableId);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_nextCableId)) = value;
		}
	}

	public unsafe float packetSpeed
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_packetSpeed);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_packetSpeed)) = value;
		}
	}

	public unsafe float spawnRateDivider
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_spawnRateDivider);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_spawnRateDivider)) = value;
		}
	}

	public unsafe string startSwitchID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startSwitchID);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startSwitchID)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string endSwitchID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endSwitchID);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endSwitchID)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe int startCustomerID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startCustomerID);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startCustomerID)) = value;
		}
	}

	public unsafe int endCustomerID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endCustomerID);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endCustomerID)) = value;
		}
	}

	public unsafe Dictionary<int, bool> cableStartPoints
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableStartPoints);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, bool>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableStartPoints)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, bool> cableEndPoints
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableEndPoints);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, bool>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableEndPoints)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Dictionary<int, List<Transform>> rawLinkTransforms
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rawLinkTransforms);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Dictionary<int, List<Transform>>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rawLinkTransforms)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe float cableWidth
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableWidth);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableWidth)) = value;
		}
	}

	public unsafe Material cableMaterial
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableMaterial);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Material>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_cableMaterial)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe float bendRadius
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bendRadius);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bendRadius)) = value;
		}
	}

	public unsafe int bendSegments
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bendSegments);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bendSegments)) = value;
		}
	}

	public unsafe float holderSegmentLength
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_holderSegmentLength);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_holderSegmentLength)) = value;
		}
	}

	public unsafe Vector3 lastPosAfterBendToPass
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastPosAfterBendToPass);
			return *(Vector3*)num;
		}
		set
		{
			*(Vector3*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastPosAfterBendToPass)) = value;
		}
	}

	public unsafe int lastCompletedCableId
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastCompletedCableId);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastCompletedCableId)) = value;
		}
	}

	public unsafe CableLink.TypeOfLink startCableLinkType
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startCableLinkType);
			return *(CableLink.TypeOfLink*)num;
		}
		set
		{
			*(CableLink.TypeOfLink*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startCableLinkType)) = value;
		}
	}

	public unsafe CableLink.TypeOfLink endtCableLinkType
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endtCableLinkType);
			return *(CableLink.TypeOfLink*)num;
		}
		set
		{
			*(CableLink.TypeOfLink*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endtCableLinkType)) = value;
		}
	}

	public unsafe string startServerID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startServerID);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_startServerID)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string endServerID
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endServerID);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_endServerID)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe float currentCableLength
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentCableLength);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentCableLength)) = value;
		}
	}

	public unsafe static float totalCableLengthLaid
	{
		get
		{
			Unsafe.SkipInit(out float result);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_totalCableLengthLaid, (void*)(&result));
			return result;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_totalCableLengthLaid, (void*)(&value));
		}
	}

	static CablePositions()
	{
		Il2CppClassPointerStore<CablePositions>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "CablePositions");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<CablePositions>.NativeClassPtr);
		NativeFieldInfoPtr_instance = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "instance");
		NativeFieldInfoPtr_cables = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cables");
		NativeFieldInfoPtr_rawCablePoints = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "rawCablePoints");
		NativeFieldInfoPtr_cableEntities = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cableEntities");
		NativeFieldInfoPtr_cableGameObjects = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cableGameObjects");
		NativeFieldInfoPtr_cableMaterials = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cableMaterials");
		NativeFieldInfoPtr_nextCableId = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "nextCableId");
		NativeFieldInfoPtr_packetSpeed = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "packetSpeed");
		NativeFieldInfoPtr_spawnRateDivider = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "spawnRateDivider");
		NativeFieldInfoPtr_startSwitchID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "startSwitchID");
		NativeFieldInfoPtr_endSwitchID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "endSwitchID");
		NativeFieldInfoPtr_startCustomerID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "startCustomerID");
		NativeFieldInfoPtr_endCustomerID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "endCustomerID");
		NativeFieldInfoPtr_cableStartPoints = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cableStartPoints");
		NativeFieldInfoPtr_cableEndPoints = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cableEndPoints");
		NativeFieldInfoPtr_rawLinkTransforms = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "rawLinkTransforms");
		NativeFieldInfoPtr_cableWidth = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cableWidth");
		NativeFieldInfoPtr_cableMaterial = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "cableMaterial");
		NativeFieldInfoPtr_bendRadius = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "bendRadius");
		NativeFieldInfoPtr_bendSegments = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "bendSegments");
		NativeFieldInfoPtr_holderSegmentLength = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "holderSegmentLength");
		NativeFieldInfoPtr_lastPosAfterBendToPass = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "lastPosAfterBendToPass");
		NativeFieldInfoPtr_lastCompletedCableId = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "lastCompletedCableId");
		NativeFieldInfoPtr_startCableLinkType = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "startCableLinkType");
		NativeFieldInfoPtr_endtCableLinkType = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "endtCableLinkType");
		NativeFieldInfoPtr_startServerID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "startServerID");
		NativeFieldInfoPtr_endServerID = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "endServerID");
		NativeFieldInfoPtr_currentCableLength = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "currentCableLength");
		NativeFieldInfoPtr_totalCableLengthLaid = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, "totalCableLengthLaid");
		NativeMethodInfoPtr_Awake_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663402);
		NativeMethodInfoPtr_Start_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663403);
		NativeMethodInfoPtr_ClearAllCables_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663404);
		NativeMethodInfoPtr_LoadCable_Public_Void_CableSaveData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663405);
		NativeMethodInfoPtr_CreateNewCable_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663406);
		NativeMethodInfoPtr_CreateNewReverseCable_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663407);
		NativeMethodInfoPtr_AssignNewPosition_Public_Void_Int32_Transform_Boolean_Boolean_TypeOfLink_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663408);
		NativeMethodInfoPtr_GenerateFinalPath_Private_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663409);
		NativeMethodInfoPtr_GenerateCornerBend_Private_IEnumerable_1_Vector3_Vector3_Vector3_Vector3_Transform_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663410);
		NativeMethodInfoPtr_GenerateBentSegment_Private_IEnumerable_1_Vector3_Vector3_Vector3_Transform_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663411);
		NativeMethodInfoPtr_RedrawCable_Private_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663412);
		NativeMethodInfoPtr_CreateTubeMesh_Private_Mesh_List_1_Vector3_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663413);
		NativeMethodInfoPtr_RemovePosition_Public_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663414);
		NativeMethodInfoPtr_RemoveLastPosition_Public_Transform_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663415);
		NativeMethodInfoPtr_GetCablePositions_Public_List_1_Vector3_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663416);
		NativeMethodInfoPtr_GetRawCablePositions_Public_List_1_Vector3_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663417);
		NativeMethodInfoPtr_GetRawLinkTransforms_Public_List_1_Transform_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663418);
		NativeMethodInfoPtr_AssignEntity_Public_Void_Int32_Entity_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663419);
		NativeMethodInfoPtr_IsCableComplete_Public_Boolean_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663420);
		NativeMethodInfoPtr_GetCableMaterial_Public_Material_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663421);
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<CablePositions>.NativeClassPtr, 100663422);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24331, XrefRangeEnd = 24340, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Awake()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Awake_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24340, XrefRangeEnd = 24346, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Start()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Start_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 24389, RefRangeEnd = 24390, XrefRangeStart = 24346, XrefRangeEnd = 24389, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ClearAllCables()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ClearAllCables_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24390, XrefRangeEnd = 24430, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void LoadCable(CableSaveData cableData)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)cableData);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_LoadCable_Public_Void_CableSaveData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24430, XrefRangeEnd = 24462, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int CreateNewCable()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CreateNewCable_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24462, XrefRangeEnd = 24477, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int CreateNewReverseCable()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CreateNewReverseCable_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24477, XrefRangeEnd = 24561, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AssignNewPosition(int cableId, Transform linkTransform, bool isStartPoint = false, bool isEndPoint = false, CableLink.TypeOfLink typeOfLink = CableLink.TypeOfLink.None, string serverID = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[6];
		*ptr = (nint)(&cableId);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)linkTransform);
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &isStartPoint;
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &isEndPoint;
		*(CableLink.TypeOfLink**)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = &typeOfLink;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)5u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(serverID);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AssignNewPosition_Public_Void_Int32_Transform_Boolean_Boolean_TypeOfLink_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 24608, RefRangeEnd = 24609, XrefRangeStart = 24561, XrefRangeEnd = 24608, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void GenerateFinalPath(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GenerateFinalPath_Private_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 24648, RefRangeEnd = 24649, XrefRangeStart = 24609, XrefRangeEnd = 24648, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe IEnumerable<Vector3> GenerateCornerBend(Vector3 p_prev, Vector3 p_curr, Vector3 p_next, Transform t_curr)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = (nint)(&p_prev);
		*(Vector3**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &p_curr;
		*(Vector3**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &p_next;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)t_curr);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GenerateCornerBend_Private_IEnumerable_1_Vector3_Vector3_Vector3_Vector3_Transform_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<IEnumerable<Vector3>>(intPtr2) : null;
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 24679, RefRangeEnd = 24681, XrefRangeStart = 24649, XrefRangeEnd = 24679, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe IEnumerable<Vector3> GenerateBentSegment(Vector3 connectionPoint, Vector3 nextPoint, Transform linkTransform, bool isStart)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = (nint)(&connectionPoint);
		*(Vector3**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &nextPoint;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)linkTransform);
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &isStart;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GenerateBentSegment_Private_IEnumerable_1_Vector3_Vector3_Vector3_Transform_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<IEnumerable<Vector3>>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 24731, RefRangeEnd = 24734, XrefRangeStart = 24681, XrefRangeEnd = 24731, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RedrawCable(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RedrawCable_Private_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 24844, RefRangeEnd = 24845, XrefRangeStart = 24734, XrefRangeEnd = 24844, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Mesh CreateTubeMesh(List<Vector3> path)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)path);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CreateTubeMesh_Private_Mesh_List_1_Vector3_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Mesh>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24845, XrefRangeEnd = 24883, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemovePosition(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemovePosition_Public_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 24924, RefRangeEnd = 24925, XrefRangeStart = 24883, XrefRangeEnd = 24924, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Transform RemoveLastPosition(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveLastPosition_Public_Transform_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Transform>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24925, XrefRangeEnd = 24931, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<Vector3> GetCablePositions(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetCablePositions_Public_List_1_Vector3_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Vector3>>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 24935, RefRangeEnd = 24936, XrefRangeStart = 24931, XrefRangeEnd = 24935, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<Vector3> GetRawCablePositions(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetRawCablePositions_Public_List_1_Vector3_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Vector3>>(intPtr2) : null;
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 24940, RefRangeEnd = 24942, XrefRangeStart = 24936, XrefRangeEnd = 24940, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<Transform> GetRawLinkTransforms(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetRawLinkTransforms_Public_List_1_Transform_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Transform>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24942, XrefRangeEnd = 24950, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AssignEntity(int cableId, Entity entity)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&cableId);
		*(Entity**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &entity;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AssignEntity_Public_Void_Int32_Entity_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 24963, RefRangeEnd = 24970, XrefRangeStart = 24950, XrefRangeEnd = 24963, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsCableComplete(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsCableComplete_Public_Boolean_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 24973, RefRangeEnd = 24974, XrefRangeStart = 24970, XrefRangeEnd = 24973, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Material GetCableMaterial(int cableId)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&cableId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetCableMaterial_Public_Material_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Material>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 24974, XrefRangeEnd = 25027, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe CablePositions()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<CablePositions>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public CablePositions(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
