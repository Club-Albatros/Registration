Registration Module
===================

This is a module for DNN 7.1.2 and higher. The reason I made it is because I'm struggling with a number of challenges users have in registering on a website I manage. These are the main objectives I have tried to accomplish here:

- The ability to sign on to specific roles (determined in the settings of the module)
- The ability to change the fields that the user sees and/or must complete depending on the selected roles ("Field Chooser" on the module's menu)
- The ability to override the name of these roles ("Role Editor" on the module's menu)
- Country now autocomplete instead of dropdown
- Country change will provoke regions to change but without postback
- City now autocomplete taken from those cities that have already been submitted

The code is organized in a way to stay close to the current registration logic. That is, the fields you define in the user profile settings are used as a basis. The categories are also used (although they are not content localized). The module further assumes we store the country and region as *code* not as the (english) plain text value.

Status
======

This is mostly a *study* to see how we could do something like this in DNN and meant to drive discussions within the DNN MVP team.

Enjoy
