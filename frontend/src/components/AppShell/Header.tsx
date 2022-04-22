import { Image, Header as HeaderComponent } from "@mantine/core";
import React from "react";

function Header() {
  return (
    <div style={{ zIndex: 20, display: "fixed", position: "sticky", top: 0 }}>
      <HeaderComponent style={{}} height={80} p="xs">
        {
          <div
            style={{
              width: 150,
              height: 60,
            }}
          >
            <Image
              radius="md"
              src="https://www.zion-net.co.il/wp-content/uploads/2021/06/ZioNET-Logo_new_new-300x127.png"
              alt="Zionet Logo"
            />
          </div>
        }
      </HeaderComponent>
    </div>
  );
}

export default Header;
