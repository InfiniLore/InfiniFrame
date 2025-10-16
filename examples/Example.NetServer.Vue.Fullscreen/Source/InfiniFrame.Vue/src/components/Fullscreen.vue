<script lang="ts" setup>
import {ref} from 'vue';

const isFullscreen = ref(false);

function toggleFullscreen() {
    if (!document.fullscreenElement) {
        document.body.requestFullscreen().then(() => {
            isFullscreen.value = true;
        }).catch((err) => {
            console.error(`Error attempting to enable full-screen mode: ${err.message} (${err.name})`);
        });
    } else if (document.exitFullscreen) {
        document.exitFullscreen().then(() => {
            isFullscreen.value = false;
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
