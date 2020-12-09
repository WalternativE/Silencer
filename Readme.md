# Silencer™ - Keep your Twitter DMs Clean

This project was built for the [Virtual ML.NET Hackathon (2020 Edition)](https://github.com/virtualmlnet/hackathon-2020). All the bits are working but be aware, that we're working with bleeding edge software on very unsupported use cases ⚠ Other than that be free to try it out, clone it, fork it and play with it. The tech is great and the future of machine learning in .NET, on the edge and in the web is bright and shiny 🦄

## How to get it?

Download the [current release](https://github.com/WalternativE/Silencer/releases/tag/0.1.0), unpack the `.crx` and load the extension into your browser (unbundled). Be aware, that the extension - as of writing this - only works reliably with [Brave](https://brave.com/).

## What is it?

Silencer™ is a Chrome Browser Extension, that starts to get active whenever you open up a message thread within your Twitter DMs. It scans the page for messages (and observes the thread for new messages), extracts the text, infers whether the text could be toxic and - in case it decides, that the text is just too rude - changes the text to a warning disclaimer.

The inference process is done using a `ML.NET` model running directly in the Browser via Bolero (WebAssembly), which is itself built on top of [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor).

## Demo Video

As this hackathon is fully remote there is no fun presentation which shows the prototype in all its unstable and hacky glory. So, instead of watching me point at my computer, please enjoy the demo video [on YouTube](https://youtu.be/FIKfwZ34KFI).

## Used components

As usual I've been standing (running, even) on the shoulders of giants to build this fun little project. You'll find the following components:

- [.NET 5](https://dotnet.microsoft.com/)
- [F# 5](https://fsharp.org/)
- [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet)
- [Bolero](https://fsbolero.io/)
- [Fake](https://fake.build/)
- [Paket](https://fsprojects.github.io/Paket/)

All development was done using:

- [VSCode Insiders](https://code.visualstudio.com/insiders/)
- [Ionide .NET 5 Preview](https://github.com/ionide/ionide-vscode-fsharp/issues/1305#issuecomment-726854574) - If you love Ionide as much as I do you should [become a sponsor](https://opencollective.com/ionide) and be nice to [Krzysztof](https://twitter.com/k_cieslak)

For my first steps at understanding the used dataset I used the following projects:

- [.NET Interactive](https://github.com/dotnet/interactive)
- [Plotly.NET](https://plotly.github.io/Plotly.NET/index.html)
- [Deedle](https://fslab.org/Deedle/)

## How to build

Make sure you have a .NET SDK version installed which corresponds to the one pinned in the `global.json` file within this repository. Then execute the following commands:

- `dotnet tool restore` to get the build tools `fake` and `paket`
- `dotnet paket install` to fix the build dependencies (fake isn't totally in the .NET 5 world yet and needs this extra step)
- `dotnet fake build -t buildrelease` to build the extension and format it in away that Brave can work with it

## Data and Modelling

I used the [Cleaned Toxic Comments dataset](https://www.kaggle.com/fizzbuzz/cleaned-toxic-comments) to perform a logistic regression with `ML.NET`. It infers whether a comment is toxic or not. You can inspect the way I played around with the data and build the model in the accompanying [.NET Interactive notebook](notebooks/ToxicCommentsModel.ipynb). Please be aware that all used paths in the notebook are absolute because of some limitations of using .NET Interactive from within VSCode. Change them according to your own setup.

## Known Limitations

- ~~You have to refresh the browser in the DM or navigate directly using the URL~~
    + ~~This problem is solvable using different listeners within the content script~~
- Loads of processing done when moving between DM threads or scrolling fast
    + I'd need to debounce the events here before I send them over to `ML.NET`
- ~~Observer stays observing when the DM threads are left~~
    + ~~Could be resolved by listening to PWA typical navigation events. Similar to the first known limitation in this list. I should generally clean up my observers.~~
- Only works reliably in Brave
    + This is due to the way the popup UI, the background Bolero app and the content script are communicating. It is programmed for the same spec but every Chromium based Browser has its own quirks. Should be resolvable for all major Browsers though.
    + Bug sometimes also appears in Brave but rather seldomly
- Initial load is very slow
    + Currently the `ML.NET` model is loaded when the background Bolero application initializes. This is unfortunately blocking (within the context of the extension). Currently I would not be aware of a way to improve this.