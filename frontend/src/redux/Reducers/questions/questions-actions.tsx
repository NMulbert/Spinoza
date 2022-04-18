import * as questionsTypes from "./questions-types";

export const loadQuestions = (questions: any) => {
  return {
    type: questionsTypes.LOAD_QUESTIONS,
    questions: questions,
  };
};
