// Voice chat functionality using WebRTC
window.voiceChat = (function () {
    let dotNetReference = null;
    let localStream = null;
    let audioContext = null;
    let analyser = null;
    let isConnected = false;
    let isMuted = true;
    let gameId = null;
    let playerId = null;
    let playerName = null;

    // Initialize the voice chat component
    function initialize(dotNetRef) {
        dotNetReference = dotNetRef;
        console.log('Voice chat initialized');
    }

    // Connect to voice chat
    async function connect(gId, pId, pName) {
        try {
            gameId = gId;
            playerId = pId;
            playerName = pName;

            // Request microphone access
            localStream = await navigator.mediaDevices.getUserMedia({
                audio: {
                    echoCancellation: true,
                    noiseSuppression: true,
                    autoGainControl: true
                }
            });

            // Create audio context for analyzing audio levels
            audioContext = new (window.AudioContext || window.webkitAudioContext)();
            const source = audioContext.createMediaStreamSource(localStream);
            analyser = audioContext.createAnalyser();
            analyser.fftSize = 256;
            source.connect(analyser);

            // Start monitoring audio levels
            monitorAudioLevels();

            // Initially muted
            localStream.getAudioTracks().forEach(track => {
                track.enabled = false;
            });

            isConnected = true;
            console.log('Connected to voice chat');
            return true;
        } catch (error) {
            console.error('Error connecting to voice chat:', error);
            if (dotNetReference) {
                dotNetReference.invokeMethodAsync('OnError', 
                    'Impossibile accedere al microfono. Verifica i permessi del browser.');
            }
            return false;
        }
    }

    // Disconnect from voice chat
    function disconnect() {
        if (localStream) {
            localStream.getTracks().forEach(track => track.stop());
            localStream = null;
        }

        if (audioContext) {
            audioContext.close();
            audioContext = null;
        }

        isConnected = false;
        isMuted = true;
        console.log('Disconnected from voice chat');
    }

    // Toggle mute state
    function toggleMute(mute) {
        if (localStream) {
            localStream.getAudioTracks().forEach(track => {
                track.enabled = !mute;
            });
            isMuted = mute;
            console.log('Mute state:', mute);
        }
    }

    // Monitor audio levels to detect speaking
    function monitorAudioLevels() {
        if (!analyser || !isConnected) return;

        const dataArray = new Uint8Array(analyser.frequencyBinCount);
        let speakingTimeout = null;
        let isSpeaking = false;

        function checkAudioLevel() {
            if (!isConnected) return;

            analyser.getByteFrequencyData(dataArray);
            const average = dataArray.reduce((a, b) => a + b) / dataArray.length;

            // Threshold for detecting speech (adjust as needed)
            const speakingThreshold = 20;

            if (average > speakingThreshold && !isMuted) {
                if (!isSpeaking) {
                    isSpeaking = true;
                    if (dotNetReference) {
                        dotNetReference.invokeMethodAsync('OnParticipantSpeaking', playerName, true);
                    }
                }

                // Reset the timeout
                clearTimeout(speakingTimeout);
                speakingTimeout = setTimeout(() => {
                    isSpeaking = false;
                    if (dotNetReference) {
                        dotNetReference.invokeMethodAsync('OnParticipantSpeaking', playerName, false);
                    }
                }, 500);
            }

            requestAnimationFrame(checkAudioLevel);
        }

        checkAudioLevel();
    }

    // Public API
    return {
        initialize: initialize,
        connect: connect,
        disconnect: disconnect,
        toggleMute: toggleMute
    };
})();
