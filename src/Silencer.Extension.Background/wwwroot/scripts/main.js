chrome.runtime.onInstalled.addListener(function () {
    chrome.storage.sync.set({ color: '#3aa757' }, function () {
        console.log('Hello from within the background');
    });

    chrome.declarativeContent.onPageChanged.removeRules(undefined, function () {
        chrome.declarativeContent.onPageChanged.addRules([{
            conditions: [new chrome.declarativeContent.PageStateMatcher({
                pageUrl: { hostEquals: 'developer.chrome.com' },
            })
            ],
            actions: [new chrome.declarativeContent.ShowPageAction()]
        }]);
    });
});

chrome.runtime.onMessage.addListener(
    function (request, sender, sendResponse) {
        console.log(sender.tab ?
            "from a content script:" + sender.tab.url :
            "from the extension");
        if (request.statement) {
            console.log(`Got a message saying ${request.statement}. Attempting to call dotnet function.`)
            DotNet.invokeMethodAsync('Silencer.Extension.Background', 'NotifyFromBackgroundJs', request.statement)
                .then(m => sendResponse({ farewell: m }));

            return true;
        }
    });