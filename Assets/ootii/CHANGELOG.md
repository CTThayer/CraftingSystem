# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [2.806.0] - 2020-03-28
###Changed
- [Framework] Added "InitialSize" parameter to static GameObjectPool Initialize() and Allocate() methods, with a default value of 5.
- [SHMP] BasicShooterReload.GetAmmoQuantity() is now virtual
- [AC] Updated ActorController.cs so that it stores a copy of the Body Shape definitions in Start(), so that the original body shapes can
be restored after being removed. Added public CreateBodyShapes() and RestoerBodyShapes() methods.
- [SCMP] Extended the width and maximumn height of the info panels in the Spell Editor, as some fields were unreadable due to being truncated.

###Fixed
- [AC] Added null Transform check in property setters to avoid design-time exceptions.
- [MC] Some options in the BasicInteraction inspector were incorrectly hidden when "Use Raycast" was unchecked.
- [MP] Fixed an issue where Mount Points were scaling with the parent object when they should not (Courtesy of Thoranar).
- [SHMP] Impact decals were not being released from the object pool. Fixed (in BasicBullet.cs).
- [SCMP] Added missing "effect" motions to NPCs in demo (Sleep, Frozen, Cower, Stunned, Knocked Down)

## [2.805.0] - 2020-01-04
###Added
- [EI] Added some missing changes for EasyInput to allow the use of Unity Events.

###Changed
- [SSMP] Cleaned up code in BasicMeleeAttack.cs, AttackStyle.cs and AttackProfileEditor.cs


###Fixed
- [AC] Added missing float type serialization in BodyShape.cs.
- [MC] Changed the input alias on Sneak_v2 motion from "ChangeStance" to "Change Stance", to match the values set by the setup scripts.