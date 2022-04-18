import reducerTests from "./Reducers/tests/tests-reducer";
import { combineReducers } from "redux";
import reducerQuestions from "./Reducers/questions/questions-reducer";

const allReducers = combineReducers({
  tests: reducerTests,
  questions: reducerQuestions,
});

export default allReducers;
