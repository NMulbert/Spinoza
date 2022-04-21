import * as testsTypes from "./tests-types";

export const loadTests = (tests: any) => {
  return {
    type: testsTypes.LOAD_TESTS,
    tests: tests,
  };
};
