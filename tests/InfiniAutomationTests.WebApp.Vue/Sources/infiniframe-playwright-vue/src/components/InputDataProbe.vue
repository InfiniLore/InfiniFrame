<script lang="ts" setup>
withDefaults(defineProps<{
    title: string
    buttonText: string
    modelValue: string
    titleId?: string
    buttonId?: string
    dataInputId?: string
    dataLabel?: string
}>(), {dataLabel: 'Input data'})

const emit = defineEmits<{
    'update:modelValue': [value: string]
    submitted: [value: string]
}>()
</script>

<template>
    <v-card variant="outlined" class="pa-4">
        <v-card-title :id="titleId" class="input-data-probe-title">{{ title }}</v-card-title>
        <v-text-field :id="dataInputId" :label="dataLabel" :model-value="modelValue" variant="outlined" @update:model-value="emit('update:modelValue', $event)"/>
        <v-btn :id="buttonId" color="primary" variant="flat" @click="emit('submitted', modelValue)">{{ buttonText }}</v-btn>
    </v-card>
</template>
