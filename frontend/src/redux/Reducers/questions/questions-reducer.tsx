import * as questionsTypes from "./questions-types";

const reducerQuestions = (state: any, action: any) => {
  switch (action.type) {
    case questionsTypes.LOAD_QUESTIONS:
      return { ...state, questions: action.questions };
    default:
      return state ?? {};
  }
};

export default reducerQuestions;
