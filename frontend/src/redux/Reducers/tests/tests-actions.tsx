import * as testsTypes from "./tests-types";

export const loadTests = (tests: any) => {
  console.log("object01");
  return {
    type: testsTypes.LOAD_TESTS,
    tests: tests,
  };
};
