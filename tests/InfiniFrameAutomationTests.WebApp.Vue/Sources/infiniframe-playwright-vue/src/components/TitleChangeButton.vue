<script setup lang="ts">

import {ref} from "vue";

const isToggledToNewTitle = ref(false);
const oldTitle = ref("");

function toggleTitle() {
    if (!isToggledToNewTitle.value) {
        oldTitle.value = document.title;
        document.title = "New Title";
        isToggledToNewTitle.value = true;
    } else {
        document.title = oldTitle.value;
        oldTitle.value = "";
        isToggledToNewTitle.value = false;
    }
}
</script>

<template>
    <button
        id="title-toggle-button"
        @click="toggleTitle"
    >
        <span v-if="!isToggledToNewTitle">Define New Title</span>
        <span v-else>Reset Title</span>
    </button>

</template>

<style scoped>

</style>