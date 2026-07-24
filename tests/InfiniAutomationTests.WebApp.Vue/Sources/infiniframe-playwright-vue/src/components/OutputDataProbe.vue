<script lang="ts" setup>
withDefaults(defineProps<{
    title: string
    buttonText?: string
    data?: string
    titleId?: string
    buttonId?: string
    dataInputId?: string
    dataLabel?: string
}>(), {buttonText: 'Read data', data: '', dataLabel: 'Output data'})

const emit = defineEmits<{readRequested: []}>()
</script>

<template>
    <v-card variant="outlined" class="pa-4">
        <v-card-title :id="titleId" class="output-data-probe-title">{{ title }}</v-card-title>
        <v-text-field :id="dataInputId" :label="dataLabel" :model-value="data" variant="outlined" readonly/>
        <v-btn :id="buttonId" color="primary" variant="flat" @click="emit('readRequested')">{{ buttonText }}</v-btn>
    </v-card>
</template>
