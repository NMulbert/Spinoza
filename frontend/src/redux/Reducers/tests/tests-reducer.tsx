import * as testsTypes from "./tests-types";

const reducerTests = (state: any, action: any) => {
  switch (action.type) {
    case testsTypes.LOAD_TESTS:
      return { ...state, tests: action.tests };
    case testsTypes.LOAD_TEST:
      return { ...state, test: action.test };
    case testsTypes.ADD_TEST:
      return {
        ...state,
        tests: action.tests,
      };
    case testsTypes.UPDATE_TEST:
      return {
        ...state,
        test: action.test,
      };
    default:
      return state ?? {};
  }
};

export default reducerTests;
