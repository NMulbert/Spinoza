import { AppShell, Header, Image } from "@mantine/core";
import { type } from "os";
import React, { useEffect, useState } from "react";
import { Sidebar } from "../AppShell/SideBar";
import AllQuestionsPage from "../Questions/AllQuestionsPage";
import AllTestsPage from "../Tests/AllTestsPage";
import CreateTest from "../Tests/CreateTest";

function Dashboard() {
  const [active, setActive] = useState("Tests");
  let Content = <AllTestsPage active={active} setActive={setActive} />;

  switch (active) {
    case "Tests":
      Content = <AllTestsPage active={active} setActive={setActive} />;
      break;
    case "Questions":
      Content = <AllQuestionsPage />;
      break;
    case "CreateTest":
      Content = <CreateTest />;
      break;
  }

  return (
    <div>
      <AppShell
        header={
          <Header height={80} p="xs">
            {
              <div style={{ width: 150, height: 60, display: "fixed" }}>
                <Image
                  radius="md"
                  src="https://www.zion-net.co.il/wp-content/uploads/2021/06/ZioNET-Logo_new_new-300x127.png"
                  alt="Zionet Logo"
                />
              </div>
            }
          </Header>
        }
        padding="md"
        fixed
        navbar={<Sidebar active={active} setActive={setActive} />}
      >
        {Content}
      </AppShell>
    </div>
  );
}

export default Dashboard;
