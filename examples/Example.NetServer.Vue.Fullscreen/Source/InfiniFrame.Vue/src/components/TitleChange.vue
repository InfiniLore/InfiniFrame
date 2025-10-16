<script setup lang="ts">

function setRandomTitle() {
    const target = document.querySelector('title');
    if (!target) return;

    target.textContent = Math.random().toString(36);
}

</script>

<template>
    <button
        @click="setRandomTitle"
    >
        <span>Set random title</span>
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