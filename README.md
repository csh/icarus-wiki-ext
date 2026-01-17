# Icarus Wiki Search

A simple [Command Palette] extension for quickly searching the ICARUS [wiki].

## Features

There are two methods for invoking the wiki search currently.

### Manual Invocation

Can be keybound in extension settings for quick access to wiki search.

![command](https://github.com/user-attachments/assets/5cf8046f-4074-4877-9426-93bdd1358810)

### "Quick Search" Fallback

Quick search cannot be keybound as it is intended to operate on the input in the command palette.

You can disable this behaviour via extension settings if you would rather use manual invocation or simply find this behaviour undesirable.

![fallback](https://github.com/user-attachments/assets/805d61e7-dc37-45f2-89b0-54339e86593c)

### How can I adapt this to my wiki?

Adjusting the `apiEndpoint` parameter when instantiating `SiteOptions` is sufficient to add support for any MediaWiki based wiki you want to use this with.

The MediaWiki installation must support opensearch.

### Disclaimer

The ICARUS game icon used in this extension is the property of [RocketWerkz](https://rocketwerkz.com/) and is used for identification purposes only. All rights to the ICARUS game, its logo, and related trademarks belong to their respective owners.

This extension is an unofficial, fan-made tool and is not affiliated with, endorsed by, or associated with RocketWerkz or the ICARUS game developers.

**Icon Usage**: The icon is used under the principle of fair use for the sole purpose of identifying the ICARUS game within this software extension. No copyright infringement is intended.


[Command Palette]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/overview
[wiki]: https://icarus.fandom.com/wiki/Icarus_Wiki
