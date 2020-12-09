(function() {
    const disclaimerLine = "⚡⚡⚡BEWARE THIS WAS MOST LIKELY RUDE AND HAS BEEN SILENCED⚡⚡⚡ "

    const checkForToxicity = (text, node) => {
        chrome.runtime.sendMessage({ statement: text },
            function (response) {
                console.log(response);
                if (response.sentimentResult.toxic) {
                    node.innerHTML = disclaimerLine;
                }
            });
    };

    const checkAllVisibleMessages = () => {
        const dmSpans = document.querySelectorAll('div[role="presentation"] div span');

        dmSpans.forEach(span => {
            const text = span.innerText;
            // very meager hack against endless recursion and emoji spans
            if (!text.startsWith(disclaimerLine) && text.length > 1) {
                console.log(text);
                checkForToxicity(text, span);
            }
        });
    };

    const checkForToxicMessagesAndObserve = () => {
        checkAllVisibleMessages();

        // Because we need to watch the page we use Mutation Observers
        // I lifted this directly from MDN 💪
        // https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver
        // Select the node that will be observed for mutations
        const targetNode = document.body;

        // Options for the observer (which mutations to observe)
        const config = { attributes: true, childList: true, subtree: true };

        // Callback function to execute when mutations are observed
        const callback = function (mutationsList, observer) {
            // Use traditional 'for loops' for IE 11
            for (const mutation of mutationsList) {
                if (mutation.type === 'childList') {
                    console.log('A child node has been added or removed.');
                    checkAllVisibleMessages();
                }
            }
        };

        // Create an observer instance linked to the callback function
        const observer = new MutationObserver(callback);

        // Start observing the target node for configured mutations
        observer.observe(targetNode, config);

        return observer;
    };

    window.silencerObserver = null;
    window.silencerInterval = null;

    setTimeout(() => {
        // we crash and burn if we do this right away so we wait for two seconds
        console.log('Starting after 1500 millis');

        window.silencerInterval = setInterval(() => {
            if (window.location.href.includes('messages') && !window.silencerObserver) {
                console.log('no observer found - setting up new observer');
                window.silencerObserver = checkForToxicMessagesAndObserve();
            } else if (!window.location.href.includes('messages') && window.silencerObserver) {
                console.log('not in messages anymore - getting rid of observer');
                window.silencerObserver.disconnect();
                window.silencerObserver = null;
            }
        }, 1500);
    }, 1500);
})();
