# 🔒 **Keypad Prefab made with Udon for VRChat worlds**

![keypad](https://user-images.githubusercontent.com/93742413/226081439-83517c55-0125-4e94-914a-80504318388e.png)

# Currently being ported to VCC!

This is a fork of the known keypad from Foorack, I'm currently porting it to be used on par with [UwUtils](https://github.com/Reava/UwUtils) or standalone, with the latest Udon goodies (Like Remote String Loading) and in general, just a big refresh of it whilst keeping it mostly similar.

! **I'm also rewriting this readme as I'm changing/updating things.**

This is a drag-and-drop Keypad/Passcode Prefab for VRChat worlds made in Unity **2019.4.31f1** and **SDK3** with Udon. This prefab requires no coding from your part and is very easy to setup. Password and target door are both easily configurable, with optional support for custom activation scripts if wanting more advanced activations.

## **📥 Download:**

**Note:** The Keypad has been rewritten into UdonSharp. Don't worry! You don't have to touch UdonSharp code I promise!

1. Make sure you have UdonSharp installed via the VCC (Projects > Manage project > Plus icon on the right of Udonsharp)

[Download latest](https://github.com/Reava/U-Keypad/releases/latest/)

## **✨ Setup Tutorial**

**Settings:** Look at the settings provided on the **main Keypad object:**

![Settings available in the Keypad prefab](https://blog.foorack.com/content/images/2021/04/bild.png "Settings available in the Keypad prefab")

The main focus is "Door Object" (marked in green) which accepts any GameObject and will toggle active status depending on passcode status, and "Solution" (marked in yellow) which accepts any numeric passcode up to 8 numbers long.

"Allow List" means the usernames on that list will always be allowed no matter what code they press or no code at all. "Deny List" means those users will never be allowed, even if they type the correct code.

"Additional Solutions" are additional codes that will also be accepted, and will unlock all doors. "Additional Door Objects" is a way to provide if you have more than 1 door object, and you want to open them all at the same time.

"Key Separation" is a special mode which requires you to have the same amount of solutions as doors. When enabled it pairs each solution to its own unique door. This means solution 1 will open only door 1, solutions 2 will open only door 2, etc...

## **🖌️ Customisation!**

The new version of Keypad supports many customization features. For example translating the Keypad into your own language, by changing the status texts. You can also make the door show the door instead of hiding the door by de-selecting the "Hide Door On Granted" checkbox. 

You can disable debugging if you know what you are doing. This will make it less verbose in console, but it is recommended to leave this on. You can also change the values of the buttons to letters, add more buttons, or change the design, they are just Unity cubes...

## **⚙️ Advanced: Solution Scripting**

This is optional, and only recommended for people who are interested in doing Udon programming. You should at least have watched Tupper's tutorial on cube-rotation before attempting this!

There are 3 possible programs which are run at different stages: at success, at failure, and at reset. Each program calls a custom event. An optional variable `keypadCode` will be set with the entered code on the target program.

| Setting name   | Event name      | Description                |
| -------------- |:---------------:| -------------------------- |
| programGranted | "keypadGranted" | Runs at successful code    |
| programDenied  | "keypadDenied"  | Runs at wrong code         |
| programClosed  | "keypadClosed"  | Runs at pressing Clear/CLR |
| -------------- |:---------------:| -------------------------- |

## **💙 Hope you enjoy it!**

You are free to use this prefab without having to credit me. But if you do use it, I would love it if you sent a quick screenshot. It really gives motivation to continuously update and improve this, as well as continue making other stuff public. Thank you!

[![Discord](https://img.shields.io/badge/Discord-Foo's%20Udon%20Laboratory-blueviolet?logo=discord)](https://discord.gg/7xJdWNk)
