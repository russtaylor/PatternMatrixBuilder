Pattern Matrix Model Builder
============================

This AMOS plugin is made with the intent to simplify the creation of models based on pattern matrices in SPSS. It allows pasting the copied contents of a pattern matrix into a dialog box and constructs the applicable model in AMOS.

Installation
------------
Download `PatternMatrixBuilder.dll` file from [this repository's releases](https://github.com/russtaylor/PatternMatrixBuilder/releases/latest). When the download completes, copy the dll into AMOS' `Plugins\` directory, which is normally located in `C:\Users\{accountName}\AppData\Local\AMOSDevelopment\AMOS\{AMOSVersion}\Plugins`. The `AppData` folder is hidden by default, but you can access it by either typing in the path directly or showing hidden files.

Restart AMOS, and the plugin should then show under the 'Plugins' menu in AMOS.

Usage
-----
In AMOS, select the data set that corresponds to your pattern matrix. Click Plugins > Pattern Matrix Model Builder, select your pattern matrix in SPSS, Right click > Copy. In AMOS, paste your copied matrix into the input box. Then, click 'Create Diagram'. The model may take several minutes to generate, but could be quick depending on the size of your model.
