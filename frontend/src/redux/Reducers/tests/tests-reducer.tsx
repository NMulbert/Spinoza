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
      let allButUpdatedTest = state.tests.filter(
        (el: any) => el.id !== action.test.id
      );
      return {
        ...state,
        tests: [...allButUpdatedTest, action.test],
      };
    case testsTypes.DELETE_TEST:
      let allButDeletedTest = state.tests.filter(
        (el: any) => el.id !== action.test.id,
        console.log(action.test.id)
      );
      return {
        ...state, tests: [...allButDeletedTest]
      }
    default:
      return state ?? {};
  }
};

export default reducerTests;
