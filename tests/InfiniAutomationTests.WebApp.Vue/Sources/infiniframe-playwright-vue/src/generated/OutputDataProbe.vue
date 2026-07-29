<template>
  <section class="data-probe">
    <h2 class="output-data-probe-title" :id="titleId">{{ title }}</h2>
    <label class="data-probe-field"
      ><span>{{ dataLabel || "Output data" }}</span
      ><input :id="dataInputId" :value="data" :readOnly="true" /></label
    ><button :id="buttonId" @click="async (event) => onReadRequested()">
      {{ buttonText }}
    </button>
  </section>
</template>

<script setup lang="ts">
// @ts-nocheck
export interface OutputDataProbeProps {
  title: string;
  buttonText: string;
  data: string;
  onReadRequested: () => void;
  titleId?: string;
  buttonId?: string;
  dataInputId?: string;
  dataLabel?: string;
}

const props = defineProps<OutputDataProbeProps>();
</script>