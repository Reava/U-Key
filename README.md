
<div align=left>
  <img alt="GitHub Workflow Status" src="https://img.shields.io/github/actions/workflow/status/reava/U-Key/release.yml?style=for-the-badge">
  <img alt="GitHub" src="https://img.shields.io/github/license/Reava/U-Key?color=blue&style=for-the-badge">
  <a href="https://github.com/Reava/U-Key/releases/latest/"><img alt="GitHub release (latest by date)" src="https://img.shields.io/github/v/release/reava/U-Key?logo=unity&style=for-the-badge"></a>
  <a href="https://github.com/Reava/U-Key/releases/latest/"><img alt="GitHub all releases" src="https://img.shields.io/github/downloads/reava/U-Key/total?color=blue&style=for-the-badge"></a>
</div>

# 🔒 **Keypad Prefab made with Udon for VRChat worlds**

![keypad](https://user-images.githubusercontent.com/93742413/226099076-105fcbdf-e777-49a4-bdfc-deebb8ce7709.png)

# Easy to use & Drag and Drop

This is a drag-and-drop Keypad/Passcode Prefab for VRChat worlds made in Unity **2019.4.31f1** and **SDK3** with Udon. 
**This prefab requires no coding from your part and is very easy to setup.**
Password and target door are both easily configurable, with optional support for custom activation scripts if wanting more advanced functions.

There are many ways of customizing the keypad, from many options, audio feedback, automatic granting, teleport on grant, changing the text displayed, easily customizable UI, expandable features via the program relays, remote string loading, etc!
There is also extensive debugging done with the logs to allow easy understanding of its behaviour and fast support in case you encounter an issue.

## **📥 Download:**

**Note:** The Keypad has been written using UdonSharp but don't worry! You don't have to touch UdonSharp code I promise!

1. Make sure you have VRC World SDK 3.7.4 or higher installed via the [VCC](https://vcc.docs.vrchat.com/) 
2. Download & Import the keypad into your project
3. Enjoy?

=> [Download latest](https://github.com/Reava/U-Keypad/releases/latest/)

## **✨ Setup Tutorial**

**Settings:** Look at the settings provided on the **main Keypad object:**

![Settings available in the Keypad prefab](https://user-images.githubusercontent.com/93742413/226104995-532ec22e-09ca-46fe-8c5f-9d0200f30059.PNG)

The main focus is "Door Objects" (marked in green) which accepts any GameObject and will toggle active status depending on passcode status, and "Solution" (marked in yellow) which accepts any numeric passcode up to 8 numbers long.

"Allow List" means the usernames on that list will always be allowed no matter what code they press or no code at all. "Deny List" means those users will never be allowed, even if they type the correct code.

"Additional Solutions" are additional codes that will also be accepted, and will unlock all doors. "Additional Door Objects" is a way to provide if you have more than 1 door object, and you want to open them all at the same time.

"Key Separation" is a special mode which requires you to have the same amount of solutions as doors. When enabled it pairs each solution to its own unique door. This means solution 1 will open only door 1, solutions 2 will open only door 2, etc...

## **🖌️ Customisation!**

The Keypad supports translating the display output, custom characters to replace the numbers, and many more things!

## **⚙️ Advanced: Solution Scripting**

This is optional, and only recommended for people who are interested in doing Udon programming. 
**You should at least have watched Tupper's tutorial on cube-rotation before attempting this!**

There are 3 possible events sent at different stages: at success, at failure, and at reset. Each referenced UdonBehaviour will receive one of the events described. An optional variable `keypadCode` will be set with the entered code on the target program.

## **💙 Hope you enjoy it!**

You are free to use this prefab as you wish as long as you don't claim it as yours, republish as is, or resell. But if you do use it, I would love it if you sent a quick screenshot or credit. It really gives motivation to continuously update and improve this, as well as continue making other stuff public. Thank you!
