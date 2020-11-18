chrome.runtime.onMessage.addListener(
    function (request, sender, sendResponse) {
        console.log(sender.tab ?
            "from a content script:" + sender.tab.url :
            "from the extension");
        if (request.statement) {
            console.log(`Got a message saying ${request.statement}. Attempting to call dotnet function.`)
            DotNet.invokeMethodAsync('Silencer.Extension.Background', 'NotifyFromBackgroundJs', request.statement)
                .then(m => sendResponse({ sentimentResult: m }));

            return true;
        }
    });