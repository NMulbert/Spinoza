import reducerTests from "./Reducers/tests/tests-reducer";
import { combineReducers } from "redux";
import reducerQuestions from "./Reducers/questions/questions-reducer";
import reducerTags from "./Reducers/tags/tags-reducer";

const allReducers = combineReducers({
  tests: reducerTests,
  questions: reducerQuestions,
  tags: reducerTags,
});

export default allReducers;
