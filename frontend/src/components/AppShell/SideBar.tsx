import { createStyles, Navbar, Avatar, Image } from "@mantine/core";
import { useState } from "react";
import { Link, Outlet } from "react-router-dom";
import {
  Notes,
  QuestionMark,
  Logout,
  SwitchHorizontal,
  User,
} from "tabler-icons-react";

const useStyles = createStyles((theme, _params, getRef) => {
  const icon = getRef("icon");
  return {
    header: {
      paddingBottom: theme.spacing.md,
      marginBottom: theme.spacing.md * 1.5,
      borderBottom: `1px solid ${
        theme.colorScheme === "dark"
          ? theme.colors.dark[4]
          : theme.colors.gray[2]
      }`,
    },

    footer: {
      paddingTop: theme.spacing.md,
      marginTop: theme.spacing.md,
      borderTop: `1px solid ${
        theme.colorScheme === "dark"
          ? theme.colors.dark[4]
          : theme.colors.gray[2]
      }`,
    },

    link: {
      ...theme.fn.focusStyles(),
      display: "flex",
      alignItems: "center",
      textDecoration: "none",
      fontSize: theme.fontSizes.sm,
      color:
        theme.colorScheme === "dark"
          ? theme.colors.dark[1]
          : theme.colors.gray[7],
      padding: `${theme.spacing.xs}px ${theme.spacing.sm}px`,
      borderRadius: theme.radius.sm,
      fontWeight: 500,

      "&:hover": {
        backgroundColor:
          theme.colorScheme === "dark"
            ? theme.colors.dark[6]
            : theme.colors.gray[0],
        color: theme.colorScheme === "dark" ? theme.white : theme.black,

        [`& .${icon}`]: {
          color: theme.colorScheme === "dark" ? theme.white : theme.black,
        },
      },
    },

    linkIcon: {
      ref: icon,
      color:
        theme.colorScheme === "dark"
          ? theme.colors.dark[2]
          : theme.colors.gray[6],
      marginRight: theme.spacing.sm,
    },

    linkActive: {
      "&, &:hover": {
        backgroundColor:
          theme.colorScheme === "dark"
            ? theme.fn.rgba(theme.colors[theme.primaryColor][8], 0.25)
            : theme.colors[theme.primaryColor][0],
        color:
          theme.colorScheme === "dark"
            ? theme.white
            : theme.colors[theme.primaryColor][7],
        [`& .${icon}`]: {
          color:
            theme.colors[theme.primaryColor][
              theme.colorScheme === "dark" ? 5 : 7
            ],
        },
      },
    },
  };
});

const data = [
  { link: "/tests", label: "Tests", icon: Notes },
  { link: "/questions", label: "Questions", icon: QuestionMark },
];

export function Sidebar() {
  const { classes, cx } = useStyles();
  const [active, setActive] = useState("Tests");
  const links = data.map((item) => (
    <Link
      to={item.link}
      className={cx(classes.link, {
        [classes.linkActive]: item.label === active,
      })}
      key={item.label}
      onClick={(event) => {
        setActive(item.label);
      }}
    >
      <item.icon className={classes.linkIcon} />
      <span>{item.label}</span>
    </Link>
  ));

  return (
    <div>
      <Navbar
        width={{ base: 240 }}
        height={"91.5vh"}
        p="md"
        style={{ position: "fixed" }}
      >
        <Navbar.Section className={classes.header}>
          <a
            className={cx(classes.link, {
              [classes.linkActive]: "Username" === active,
            })}
            href="Profile"
            onClick={(event) => {
              event.preventDefault();
              setActive("Username");
            }}
          >
            <User className={classes.linkIcon} />
            <span>Username</span>
          </a>
        </Navbar.Section>

        <Navbar.Section grow>{links}</Navbar.Section>

        <Navbar.Section className={classes.footer}>
          <div className={classes.link}>
            <SwitchHorizontal className={classes.linkIcon} />
            <span>Change account</span>
          </div>
          <a
            href="#"
            className={classes.link}
            onClick={(event) => event.preventDefault()}
          >
            <Logout className={classes.linkIcon} />
            <span>Logout</span>
          </a>
        </Navbar.Section>
      </Navbar>
      <Outlet />
    </div>
  );
}
