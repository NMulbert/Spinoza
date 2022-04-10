import React from "react";
import "./ChooseFromCatalog.css";

type ChooseFromCatalogProps = {
  trigger: boolean;
  setChooseFromCatalog: (boolean: boolean) => void;
};

const ChooseFromCatalog = ({
  trigger,
  setChooseFromCatalog,
}: ChooseFromCatalogProps) => {
  return trigger ? (
    <div className="chooseFromCatalog">
      <div className="innerChooseFromCatalog">
        Choose From Catalog
        <button
          onClick={() => {
            setChooseFromCatalog(false);
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
};

export default ChooseFromCatalog;
