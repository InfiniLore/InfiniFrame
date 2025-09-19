<script lang="ts" setup>
import { ref } from 'vue';

const isFullscreen = ref(false);

declare global {
    interface Window {
        chrome?: {
            webview?: {
                postMessage(message: string): void;
            };
        };
    }
}

function sendMessageToHost(message: string) {
    // Try different messaging methods for Photino.NET
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage(message);
    } else if ((window as any).external?.sendMessage) {
        (window as any).external.sendMessage(message);
    } else {
        // Fallback for development
        console.log('Message to host failed:', message);
    }
}

function toggleFullscreen() {
	if (!document.fullscreenElement) {
		document.body.requestFullscreen().then(() => {
			isFullscreen.value = true;
            sendMessageToHost("fullscreen:enter");
		}).catch((err) => {
			console.error(`Error attempting to enable full-screen mode: ${err.message} (${err.name})`);
		});
	}
	else if (document.exitFullscreen) {
        document.exitFullscreen().then(() => {
            isFullscreen.value = false;
            sendMessageToHost("fullscreen:exit");
        }).catch((err) => {
            console.error(`Error attempting to exit full-screen mode: ${err.message} (${err.name})`);
        });
	}
}
</script>

<template>
	<button
		@click="toggleFullscreen"
	>
		<span v-if="!isFullscreen">Enter Fullscreen</span>
		<span v-else>Exit Fullscreen</span>
	</button>
</template>

<style scoped>
button {
	padding: 10px;
	background-color: lightgreen;
	color: black;
	border: 2px solid darkgreen;
	border-radius: 5px;
	margin: 1rem;
}
</style>
