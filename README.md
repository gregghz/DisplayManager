# DisplayManager

DisplayManager is a command-line utility designed to simplify the management of display layouts for Windows users. With DisplayManager, you can easily switch between user defined display layouts using simple command-line commands, making it an ideal tool for users who frequently change their display setups for gaming, work, or multimedia purposes.

This has only been tested on Windows 11 but should work on Windows 10 as well.

<!--
## Installation

### Manual

1. **Download DisplayManager**: Download the latest version of DisplayManager from the [releases page](#).
2. **Extract the Files**: Extract the downloaded ZIP file to your preferred location.
3. **Add to PATH (Optional)**: For easier access, you can add the DisplayManager directory to your system's PATH environment variable.

### Choco
-->

## Building

From the root directory of this project:

    dotnet build

This will create the an executable at `bin\Debug\net8.0\DisplayManager.exe`. Copy that where ever you'd like and optionally add it to your PATH variable.

## Usage

### Saving a Display Configuration

To save your current display layout as a configuration, use the following command:

    DisplayManager.exe --save <configuration-name>

Replace `<configuration-name>` with a name for your configuration file.

### Loading a Display Configuration

To load a previously saved display configuration, use the following command:

    DisplayManager.exe --load <configuration-name>

Make sure to replace `<configuration-name>` with the name of the configuration you wish to load.

### Listing Configurations

To list all saved configurations, use:

    DisplayManager.exe --list

## Configuration Files

Configuration files are stored in the `%APPDATA%\DisplayManager\` directory as json files. These files contain the display settings saved by the user and can be manually edited if necessary.

## Contributing

We welcome contributions to DisplayManager! If you have a suggestion or bug report, please feel free to open an issue or submit a pull request on our [GitHub repository](#).

## License

DisplayManager is released under the MIT License. See the LICENSE file in our GitHub repository for more details.
