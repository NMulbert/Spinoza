import * as testsTypes from "./tests-types";

export const loadTests = (tests: any) => {
  return {
    type: testsTypes.LOAD_TESTS,
    tests: tests,
  };
};

export const loadTest = (test: any) => {
  return {
    type: testsTypes.LOAD_TEST,
    test: test,
  };
};

export const addTest = (tests: any) => {
  return {
    type: testsTypes.ADD_TEST,
    tests: tests,
  };
};

export const updateTest = (test: any) => {
  return {
    type: testsTypes.UPDATE_TEST,
    test: test,
  };
};

export const deleteTest = (test: any) => {
  return {
    type: testsTypes.DELETE_TEST,
    test: test,
  };
};
