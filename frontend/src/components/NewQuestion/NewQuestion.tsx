import React from "react";

type NewQuestionProps = {
  trigger: boolean;
  setNewQuestion: (boolean: boolean) => void;
};

function NewQuestion({ trigger, setNewQuestion }: NewQuestionProps) {
  return trigger ? (
    <div className="chooseFromCatalog">
      <div className="innerChooseFromCatalog">
        New Question
        <button
          onClick={() => {
            setNewQuestion(false);
          }}
          className="close-btn"
        >
          Close
        </button>
      </div>
    </div>
  ) : (
    <></>
  );
}

export default NewQuestion;
