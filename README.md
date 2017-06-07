# About R7.Documents

[![BCH compliance](https://bettercodehub.com/edge/badge/roman-yagodin/R7.Documents)](https://bettercodehub.com/)
[![Join the chat at https://gitter.im/roman-yagodin/R7.Documents](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/roman-yagodin/R7.Documents?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

*R7.Documents* is a redesigned version of the classic [DNN Documents](https://github.com/mitchelsellers/dnnDocuments) module.

![Screenshot](https://raw.githubusercontent.com/roman-yagodin/R7.Documents/master/images/r7_documents.png "Main module view in the edit mode")

# Changes from the original module

## End-user changes

- Multi-tabbed, popup-enabled document editing UI.
- Icon column to display file extension or document type.
- Ability to edit document dates.
- Ability to publish/unpublish documents by setting start and end publication dates.
- Publication date column (calculated from start publication date with fallback to creation date).
- If no published documents available, module hides from reqular view.
- Page-specific module presentation settings (column set / order, sorting, etc.)
- Ability to import selected documents from any other R7.Documents or DNN Documents module.
- New DNN search integration.

## Developer changes

- Module code converted from VB.NET to C#.
- New DAL 2 used instead of DAL.
- Module settings now stored in standard DNN tables.
- All obsolete / deprecated DNN API calls were refreshed.
- Enabled cross-platform development using MonoDevelop or Visual Studio Code.

# Install 

Download release package and install as usual under Host &gt; Extensions &gt; Install Extension Wizard. 

## Install notes

- R7.Documents module peacefully co-exists with original DNN Documents in the same DNN installation, on the same page, etc.
- Russian localization resources will be installed along with english ones. If you don't need this, just delete all *.ru-RU.resx files from /DesktopModules/R7.Documents/R7.Documents/App_LocalResouces folder or modify Resouces.zip in the install package accordingly.
