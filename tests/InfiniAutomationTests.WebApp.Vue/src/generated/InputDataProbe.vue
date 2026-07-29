<template>
  <section class="data-probe">
    <h2 class="input-data-probe-title" :id="titleId">{{ title }}</h2>
    <label class="data-probe-field"
      ><span>{{ dataLabel || "Input data" }}</span
      ><input
        :id="dataInputId"
        :value="data"
        @input="async (event) => onDataChanged(event.target.value)" /></label
    ><button :id="buttonId" @click="async (event) => onSubmitted(data)">
      {{ buttonText }}
    </button>
  </section>
</template>

<script setup lang="ts">
// @ts-nocheck
export interface InputDataProbeProps {
  title: string;
  buttonText: string;
  data: string;
  onDataChanged: (value: string) => void;
  onSubmitted: (value: string) => void;
  titleId?: string;
  buttonId?: string;
  dataInputId?: string;
  dataLabel?: string;
}

const props = defineProps<InputDataProbeProps>();
</script>