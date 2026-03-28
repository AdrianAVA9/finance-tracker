import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useActionSheetStore = defineStore('actionSheet', () => {
  const isNewEntryOpen = ref(false);

  function openNewEntry() {
    isNewEntryOpen.value = true;
  }

  function closeNewEntry() {
    isNewEntryOpen.value = false;
  }

  function toggleNewEntry() {
    isNewEntryOpen.value = !isNewEntryOpen.value;
  }

  return { 
    isNewEntryOpen, 
    openNewEntry, 
    closeNewEntry, 
    toggleNewEntry 
  };
});
