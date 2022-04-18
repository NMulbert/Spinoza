import * as testsTypes from "./tests-types";
interface TestsState {
  tests: [];
}
const reducerTests = (state: any, action: any) => {
  switch (action.type) {
    case testsTypes.LOAD_TESTS:
      return { ...state, tests: action.tests };
    default:
      return state ?? {};
  }
};

export default reducerTests;
