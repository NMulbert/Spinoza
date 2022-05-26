import * as tagsTypes from "./tags-types";

const reducerTags = (state: any, action: any) => {
  switch (action.type) {
    case tagsTypes.LOAD_TAGS:
      return { ...state, tags: action.tags };

    case tagsTypes.ADD_TAGS:
      return { ...state, tags: action.tags };
    default:
      return state ?? {};
  }
};

export default reducerTags;
