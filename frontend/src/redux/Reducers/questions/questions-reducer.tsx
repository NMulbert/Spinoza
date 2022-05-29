import * as questionsTypes from "./questions-types";

const reducerQuestions = (state: any, action: any) => {
  switch (action.type) {
    case questionsTypes.LOAD_QUESTIONS:
      return { ...state, questions: action.questions };
    case questionsTypes.LOAD_QUESTION:
      return { ...state, question: action.question };
    case questionsTypes.ADD_QUESTION:
      return {
        ...state,
        questions: action.questions,
      };
    case questionsTypes.UPDATE_QUESTION:
      return {
        ...state,
        question: action.question,
      };
    default:
      return state ?? {};
  }
};

export default reducerQuestions;
