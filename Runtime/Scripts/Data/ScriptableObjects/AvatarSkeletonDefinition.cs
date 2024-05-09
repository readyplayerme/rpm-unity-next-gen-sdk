using UnityEngine;
using System.Collections.Generic;

namespace ReadyPlayerMe.Runtime.Data.ScriptableObjects
{
	[System.Serializable]
	public class BoneGroup
	{
		public string GroupName;
		public string[] BonesKeys;
		public string[] BonesValues;
	}
	
    [CreateAssetMenu(fileName = "AvatarSkeletonDefinition", menuName = "Ready Player Me/Avatar Skeleton Definition", order = 1)]
    public class AvatarSkeletonDefinition : ScriptableObject
    {
	    public BoneGroup[] BoneGroups = {
		    new BoneGroup() {
			    GroupName = "Spine",
			    BonesKeys = new [] {
				    "Hips",
				    "Spine",
				    "Chest",
				    "UpperChest",
				    "Neck",
				    "Head",
			    },
			    BonesValues = new [] { "", "", "", "", "", "" },
		    },
		    new BoneGroup() {
			    GroupName = "Right Arm", 
			    BonesKeys = new [] {
				    "RightShoulder",
				    "RightUpperArm",
				    "RightLowerArm",
				    "RightHand",
			    },
			    BonesValues = new [] { "", "", "", "" },
		    },
		    new BoneGroup() {
			    GroupName = "Right Hand", 
			    BonesKeys = new [] {
				    "Right Thumb Proximal",
				    "Right Thumb Intermediate",
				    "Right Thumb Distal",
				    "Right Index Proximal",
				    "Right Index Intermediate",
				    "Right Index Distal",
				    "Right Middle Proximal",
				    "Right Middle Intermediate",
				    "Right Middle Distal",
				    "Right Ring Proximal",
				    "Right Ring Intermediate",
				    "Right Ring Distal",
				    "Right Little Proximal",
				    "Right Little Intermediate",
				    "Right Little Distal",
			    },
			    BonesValues = new [] { "", "", "", "","", "", "", "", "", "", "", "", "", "", "", "" }, 
		    },
		    new BoneGroup() {
			    GroupName = "Left Arm", 
			    BonesKeys = new [] {
				    "LeftShoulder",
				    "LeftUpperArm",
				    "LeftLowerArm",
				    "LeftHand",
			    },
			    BonesValues = new [] { "", "", "", "" },
		    },
		    new BoneGroup() {
			    GroupName = "Left Hand", 
			    BonesKeys = new [] {
				    "Left Thumb Proximal",
				    "Left Thumb Intermediate",
				    "Left Thumb Distal",
				    "Left Index Proximal",
				    "Left Index Intermediate",
				    "Left Index Distal",
				    "Left Middle Proximal",
				    "Left Middle Intermediate",
				    "Left Middle Distal",
				    "Left Ring Proximal",
				    "Left Ring Intermediate",
				    "Left Ring Distal",
				    "Left Little Proximal",
				    "Left Little Intermediate",
				    "Left Little Distal",
			    },
			    BonesValues = new [] { "", "", "", "","", "", "", "", "", "", "", "", "", "", "", "" },
		    },
		    new BoneGroup() {
			    GroupName = "Right Leg", 
			    BonesKeys = new [] {
				    "RightUpperLeg",
				    "RightLowerLeg",
				    "RightFoot",
				    "RightToes",
			    },
			    BonesValues = new [] { "", "", "", "" },
		    },
		    new BoneGroup() {
			    GroupName = "Left Leg", 
			    BonesKeys = new [] {
				    "LeftUpperLeg",
				    "LeftLowerLeg",
				    "LeftFoot",
				    "LeftToes",
			    },
			    BonesValues = new [] { "", "", "", "" },
		    },
	    };
	    
	    public Dictionary<string, string> GetHumanBones() {
		    Dictionary<string, string> humanBones = new Dictionary<string, string>();
		    
		    foreach (BoneGroup boneGroup in BoneGroups) {
			    for (int i = 0; i < boneGroup.BonesKeys.Length; i++) {
				    humanBones[boneGroup.BonesValues[i]] = boneGroup.BonesKeys[i];
			    }
		    }
		    
		    return humanBones;
	    }
    }
}
