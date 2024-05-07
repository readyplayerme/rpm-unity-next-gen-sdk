using ReadyPlayerMe.Runtime.Data.ScriptableObjects;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ReadyPlayerMe.Runtime.Loader
{
	public class SkeletonBuilder
	{
		[System.Serializable]
		public class BoneGroup
		{
			[HideInInspector] public bool Foldout;
			public string GroupName;
			public Bone[] Bones;
		}

		[System.Serializable]
		public class Bone
		{
			public string Name;
			[HideInInspector] public Transform Transform;
			public string BonePath;
		}
		
		//A static dictionary containing the mapping from joint/bones names in the model
		//to the names Unity uses for them internally.
		//In this case they match the naming from the included Mixamo model on the left
		//and the Unity equivalent name on the right. 
		//This does not need to be hard-coded. 
		private Dictionary<string, string> HumanSkeletonNames = new Dictionary<string, string>()
		{
			{ "Spine1", "Chest" },
			{ "Head", "Head" },
			{ "Hips", "Hips" },
			{ "LeftHandIndex3", "Left Index Distal" },
			{ "LeftHandIndex2", "Left Index Intermediate" },
			{ "LeftHandIndex1", "Left Index Proximal" },
			{ "LeftHandPinky3", "Left Little Distal" },
			{ "LeftHandPinky2", "Left Little Intermediate" },
			{ "LeftHandPinky1", "Left Little Proximal" },
			{ "LeftHandMiddle3", "Left Middle Distal" },
			{ "LeftHandMiddle2", "Left Middle Intermediate" },
			{ "LeftHandMiddle1", "Left Middle Proximal" },
			{ "LeftHandRing3", "Left Ring Distal" },
			{ "LeftHandRing2", "Left Ring Intermediate" },
			{ "LeftHandRing1", "Left Ring Proximal" },
			{ "LeftHandThumb3", "Left Thumb Distal" },
			{ "LeftHandThumb2", "Left Thumb Intermediate" },
			{ "LeftHandThumb1", "Left Thumb Proximal" },
			{ "LeftFoot", "LeftFoot" },
			{ "LeftHand", "LeftHand" },
			{ "LeftForeArm", "LeftLowerArm" },
			{ "LeftLeg", "LeftLowerLeg" },
			{ "LeftShoulder", "LeftShoulder" },
			{ "LeftToeBase", "LeftToes" },
			{ "LeftArm", "LeftUpperArm" },
			{ "LeftUpLeg", "LeftUpperLeg" },
			{ "Neck", "Neck" },
			{ "RightHandIndex3", "Right Index Distal" },
			{ "RightHandIndex2", "Right Index Intermediate" },
			{ "RightHandIndex1", "Right Index Proximal" },
			{ "RightHandPinky3", "Right Little Distal" },
			{ "RightHandPinky2", "Right Little Intermediate" },
			{ "RightHandPinky1", "Right Little Proximal" },
			{ "RightHandMiddle3", "Right Middle Distal" },
			{ "RightHandMiddle2", "Right Middle Intermediate" },
			{ "RightHandMiddle1", "Right Middle Proximal" },
			{ "RightHandRing3", "Right Ring Distal" },
			{ "RightHandRing2", "Right Ring Intermediate" },
			{ "RightHandRing1", "Right Ring Proximal" },
			{ "RightHandThumb3", "Right Thumb Distal" },
			{ "RightHandThumb2", "Right Thumb Intermediate" },
			{ "RightHandThumb1", "Right Thumb Proximal" },
			{ "RightFoot", "RightFoot" },
			{ "RightHand", "RightHand" },
			{ "RightForeArm", "RightLowerArm" },
			{ "RightLeg", "RightLowerLeg" },
			{ "RightShoulder", "RightShoulder" },
			{ "RightToeBase", "RightToes" },
			{ "RightArm", "RightUpperArm" },
			{ "RightUpLeg", "RightUpperLeg" },
			{ "Spine", "Spine" },
			{ "Spine2", "UpperChest" }
		};

		/// Create a HumanDescription out of an avatar GameObject. 
		/// The HumanDescription is what is needed to create an Avatar object
		/// using the AvatarBuilder API. This function takes care of 
		/// creating the HumanDescription by going through the avatar's
		/// hierarchy, defining its T-Pose in the skeleton, and defining
		/// the transform/bone mapping in the HumanBone array. 
		private HumanDescription CreateHumanDescription(GameObject avatarRoot)
		{
			HumanDescription description = new HumanDescription()
			{
				skeleton = CreateSkeleton(avatarRoot),
				human = CreateHuman(avatarRoot),
			};

			return description;
		}

		// Create a SkeletonBone array out of an Avatar GameObject
		// This assumes that the Avatar as supplied is in a T-Pose
		// The local positions of its bones/joints are used to define this T-Pose
		private SkeletonBone[] CreateSkeleton(GameObject avatarRoot)
		{
			List<SkeletonBone> skeleton = new List<SkeletonBone>();

			Transform[] avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();
			foreach (Transform avatarTransform in avatarTransforms)
			{
				SkeletonBone bone = new SkeletonBone()
				{
					name = avatarTransform.name,
					position = avatarTransform.localPosition,
					rotation = avatarTransform.localRotation,
					scale = avatarTransform.localScale
				};

				skeleton.Add(bone);
			}

			return skeleton.ToArray();
		}

		// Create a HumanBone array out of an Avatar GameObject
		// This is where the various bones/joints get associated with the
		// joint names that Unity understands. This is done using the
		// static dictionary defined at the top. 
		private HumanBone[] CreateHuman(GameObject avatarRoot)
		{
			List<HumanBone> human = new List<HumanBone>();

			Transform[] avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();
			foreach (Transform avatarTransform in avatarTransforms)
			{
				if (HumanSkeletonNames.TryGetValue(avatarTransform.name, out string humanName))
				{
					HumanBone bone = new HumanBone
					{
						boneName = avatarTransform.name,
						humanName = humanName,
						limit = new HumanLimit()
					};
					bone.limit.useDefaultValues = true;

					human.Add(bone);
				}
			}

			return human.ToArray();
		}
		
		/// <summary>
		///		Applies a skeleton to the given Avatar GameObject.
		/// </summary>
		/// <param name="source">Avatar GameObject</param>
		public void Build(GameObject source)
		{
			Transform leftArm = source.transform.Find("Armature/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm");
			Transform rightArm = source.transform.Find("Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm");
			Transform lowerLeftArm = source.transform.Find("Armature/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm");
			Transform lowerRightArm = source.transform.Find("Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm");

			leftArm.localRotation = Quaternion.Euler(0, 10, -45f);
			rightArm.localRotation = Quaternion.Euler(0, -10, 45f);
			lowerLeftArm.localRotation = Quaternion.Euler(0, 0, 0f);
			lowerRightArm.localRotation = Quaternion.Euler(0, 0, 0f);

			var description = CreateHumanDescription(source);
			Avatar avatar = AvatarBuilder.BuildHumanAvatar(source, description);

			Animator animator = source.GetComponent<Animator>();
			if (animator != null)
			{
				animator.avatar = avatar;
			}
		}

		public void Build(BoneGroup[] boneGroups, Transform avatar, Transform skeletonRoot)
		{
			// get all transforms if only not null
			Bone[] bones = boneGroups.SelectMany(bg => bg.Bones).Where(b => b.Transform != null).ToArray();
			
			// create skeleton
			List<SkeletonBone> skeleton = new List<SkeletonBone>();
			foreach (Bone abone in bones)
			{
				SkeletonBone bone = new SkeletonBone()
				{
					name = abone.Transform.name,
					position = abone.Transform.localPosition,
					rotation = abone.Transform.localRotation,
					scale = abone.Transform.localScale
				};

				skeleton.Add(bone);
			}
			
			// create human
			List<HumanBone> human = new List<HumanBone>();
			foreach (Bone abone in bones)
			{
				HumanBone bone = new HumanBone
				{
					boneName = abone.Transform.name,
					humanName = abone.Name,
					limit = new HumanLimit()
				};
				bone.limit.useDefaultValues = true;

				human.Add(bone);
			}
			
			HumanDescription description = new HumanDescription()
			{
				skeleton = skeleton.ToArray(),
				human = human.ToArray()
			};

			Avatar animAvatar = AvatarBuilder.BuildHumanAvatar(skeletonRoot.gameObject, description);
			
			AssetDatabase.CreateAsset(animAvatar, "Assets/Avatar.asset");
			
			AvatarSkeletonDefinition definition = ScriptableObject.CreateInstance<AvatarSkeletonDefinition>();

			foreach (var group in boneGroups)
			{
				foreach (var bone in group.Bones)
				{
					bone.BonePath = GetBonePath(bone.Transform, skeletonRoot);
				}
			}
			
			definition.rootBonePath = GetBonePath(skeletonRoot, avatar);
			definition.boneGroups = boneGroups;
			AssetDatabase.CreateAsset(definition, "Assets/AvatarSkeletonDefinition.asset");

			Animator animator = skeletonRoot.GetComponent<Animator>();
			if (animator != null)
			{
				animator.avatar = animAvatar;
			}
		}
		
		public void Load(AvatarSkeletonDefinition definition, BoneGroup[] boneGroups, ref Transform skeletonRoot)
		{
			foreach (var group in definition.boneGroups)
			{
				foreach (var bone in group.Bones)
				{
					Transform boneTransform = skeletonRoot.Find(bone.BonePath);
					if (boneTransform != null)
					{
						boneGroups.First(bg => bg.GroupName == group.GroupName).Bones.First(b => b.Name == bone.Name).Transform = boneTransform;
					}
				}
			}
		}
		
		public string GetBonePath(Transform transform, Transform root)
		{
			if (transform == null)
			{
				return string.Empty;
			}
            
			var path = transform.name;
			while (transform.parent != null && transform.parent != root)
			{
				transform = transform.parent;
				path = transform.name + "/" + path;
			}

			return path;
		}
	}
}
