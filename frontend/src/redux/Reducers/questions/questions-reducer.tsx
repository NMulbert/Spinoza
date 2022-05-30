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
      let allButUpdatedQuestion = state.questions.filter(
        (el: any) => el.id !== action.question.id
      );
      return {
        ...state,
        questions: [...allButUpdatedQuestion, action.question],
      };
    case questionsTypes.DELETE_QUESTION:
      let allButDeletedQuestion = state.questions.filter(
        (el: any) => el.id !== action.question.id
      );
      return {
        ...state,
        questions: [...allButDeletedQuestion],
      };
    default:
      return state ?? {};
  }
};

export default reducerQuestions;
