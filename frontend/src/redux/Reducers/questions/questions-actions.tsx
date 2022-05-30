import * as questionsTypes from "./questions-types";

export const loadQuestions = (questions: any) => {
  return {
    type: questionsTypes.LOAD_QUESTIONS,
    questions: questions,
  };
};

export const loadQuestion = (question: any) => {
  return {
    type: questionsTypes,
    question: question,
  };
};

export const addQuestion = (questions: any) => {
  return {
    type: questionsTypes.ADD_QUESTION,
    questions: questions,
  };
};

export const updateQuestion = (question: any) => {
  return {
    type: questionsTypes.UPDATE_QUESTION,
    question: question,
  };
};

export const deleteQuestion = (question: any) => {
  return {
    type: questionsTypes.DELETE_QUESTION,
    question: question,
  };
};