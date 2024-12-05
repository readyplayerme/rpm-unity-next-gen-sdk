# Avatar Creator Sample Guide

This Ready Player Me Unity sample demonstrates features of the package and provides example scenes for testing and development. Please follow the instructions below to set up the sample correctly.

---

## **Prerequisites**

- This sample uses **URP materials**. Ensure your project is using the **Universal Render Pipeline (URP)**. Attempting to use these assets in a non-URP project may result in material and shader issues.

---

## **Scenes**

### Included Scenes:
1. **Character Customizer**
    - Allows players to create and customize characters.
2. **Game Scene**
    - A sample gameplay scene demonstrating character integration.

### Setup:
To enable scene switching during runtime, **add both scenes to your Build Settings**:
1. Open `File > Build Settings`.
2. Drag and drop the `RpmAvatarCreator` and `RpmGameScene` from the `Samples/AvatarCreator/Scenes` folder into the Scenes in Build list.

---

## **Login Requirements**

Before running the scenes, you must log in to the **Ready Player Me demo account**:

1. Open the **Style Manager** window:
    - Navigate to `Tools > Ready Player Me > Style Manager` in the Unity menu bar.
2. Enter the provided demo account credentials and log in.
3. Once logged in, the sample scenes will have access to the necessary features.

---

## **Troubleshooting**

- If materials or shaders appear broken:
    - Verify that your project is using **URP**.
    - Update your URP settings and ensure the correct URP pipeline asset is assigned under `Edit > Project Settings > Graphics`.

- If scene switching does not work:
    - Double-check that both scenes are added to the Build Settings.

---

## **Contact**

For further assistance or to report issues, please contact the support team or refer to the documentation provided with the package.
