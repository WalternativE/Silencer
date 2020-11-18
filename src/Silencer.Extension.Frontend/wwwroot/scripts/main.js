window.sendGreeting = (statement) => {
    chrome.runtime.sendMessage({ statement },
        function (response) {
            console.log(response);
        });
}