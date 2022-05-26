import * as tagsTypes from "./tags-types";

export const loadTags = (tags: any) => {
  return {
    type: tagsTypes.LOAD_TAGS,
    tags: tags,
  };
};

export const addTags = (tags: any) => {
  return {
    type: tagsTypes.ADD_TAGS,
    tags: tags,
  };
};
