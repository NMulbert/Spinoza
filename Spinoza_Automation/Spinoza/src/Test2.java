import java.util.Date;
import java.util.Random;
import java.util.concurrent.TimeUnit;

import org.openqa.selenium.Alert;
import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;

public class Test2 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Tests Edit Test

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(15, TimeUnit.SECONDS);

		driver.manage().window().maximize();
		driver.get("http://localhost:3000/");

		System.out.println("TEST START - TEST'S EDIT");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 1");
		EditTestTitle(driver);
		System.out.println("TEST END - TEST 1");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 2");
		EditTestTitle_Test1(driver);
		System.out.println("TEST END - TEST 2");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 3");
		EditTestDescription(driver);
		System.out.println("TEST END - TEST 3");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 4");
		EditTestTags(driver);
		System.out.println("TEST END - TEST 4");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 5");
		EditTestQuestions_CreateNewQuestion(driver);
		System.out.println("TEST END - TEST 5");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 6");
		EditTestQuestion_ChooseFromCatalog(driver);
		System.out.println("TEST END - TEST 6");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST END - TEST'S EDIT");

		Thread.sleep(3000);
		driver.quit();

	}

	public static void EditTestTitle(WebDriver driver) {
		try {
			// EDIT TEST TITLE

			// EDIT TEST
			EditTest(driver);

			// EDIT TITLE
			WebElement Title = driver.findElement(By.xpath(
					"//input[@class='mantine-TextInput-defaultVariant mantine-TextInput-input mantine-us7vc8']"));
			Title.click();
			Title.clear();
			Title.sendKeys("NEW TITLE");
			System.out.println("TITLE = 'NEW TITLE'");

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST EDIT TITLE FAILED");
			} else {
				EditTestTitle_Check(driver);
			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestTitle");
			System.out.println(e);
		}
	}

	public static void EditTestTitle_Test1(WebDriver driver) {
		try {
			// EDIT TEST WITHOUT TITLE

			// EDIT TEST
			EditTest(driver);

			// EDIT TITLE
			WebElement Title = driver.findElement(By.xpath(
					"//input[@class='mantine-TextInput-defaultVariant mantine-TextInput-input mantine-us7vc8']"));
			Title.clear();

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			EditTestTitle_Test1_Check(driver);

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestTitle_Test1");
			System.out.println(e);
		}
	}

	public static void EditTestDescription(WebDriver driver) {
		try {
			// EDIT TEST DESCRIPTION

			// EDIT TEST
			EditTest(driver);

			// EDIT DESCRIPTION
			WebElement Description = driver.findElement(By.cssSelector(".w-md-editor-text-input"));
			Description.click();
			Description.clear();
			Description.sendKeys("NEW EDIT DESCRIPTION");
			System.out.println("DESCRIPTION = 'NEW EDIT DESCRIPTION'");

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST EDIT DESCRIPTION FAILED");
			} else {
				EditTestDescription_Check(driver);
			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestDescription");
			System.out.println(e);
		}

	}

	public static void EditTestTags(WebDriver driver) {
		try {
			// EDIT TAGS

			// EDIT TEST
			EditTest(driver);

			// EDIT TAGS
			int TagsCount = driver.findElements((By.xpath(
					"//button[@class='mantine-ActionIcon-transparent mantine-ActionIcon-root mantine-MultiSelect-defaultValueRemove mantine-9kufq0']")))
					.size();

			if (TagsCount > 1) {
				driver.findElement(By.xpath(
						"(//button[@class='mantine-ActionIcon-transparent mantine-ActionIcon-root mantine-MultiSelect-defaultValueRemove mantine-9kufq0'])[1]"))
						.click();
				System.out.println("TAGS = " + (driver
						.findElement(By.cssSelector(".mantine-1kunnmy.mantine-MultiSelect-values")).getText()));
			}

			driver.findElement(By.cssSelector(".w-md-editor-text-input")).click();

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST EDIT DESCRIPTION FAILED");
			} else {
				EditTestTags_Check(driver);
			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestTags");
			System.out.println(e);
		}
	}

	public static void EditTestQuestions_CreateNewQuestion(WebDriver driver) {
		try {
			// EDIT TEST BY CREATE NEW QUESTION

			// EDIT TEST
			EditTest(driver);

			// CREATE NEW QUESTION
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-outline mantine-Button-root mantine-Group-child mantine-1r1wfgg']"))
					.click();

			// ADD TITLE
			QuestionAddTitle(driver);

			// ADD TAGS
			QuestionAddTags(driver);

			// ADD DESCRPITION
			QuestionAddDescription(driver);

			// ADD ANSWER TYPE [1:NORMAL,2MULTIPLE]
			QuestionAddAnswerType(driver, 1);

			// ADD LEVEL
			QuestionAddSelectLevel(driver);

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"body > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(4) > div:nth-child(1) > button:nth-child(1)"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.cssSelector("div[role='alert']")));

			String alert = driver.findElement(By.cssSelector("div[role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("EDIT TEST QUESTION - CREATE NEW QUESTION = FAILED");

			} else {
				EditTestQuestions_CreateNewQuestion_Check(driver);

				// RE-ENTER TO QUESTIONS
				driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
				driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();
			}

		} catch (Exception e) {
			System.out.println("Error = EditTestQuestions_CreateNewQuestion");
			System.out.println(e);
		}
	}

	public static void EditTestQuestion_ChooseFromCatalog(WebDriver driver) {
		try {
			// EDIT TEST BY CHOSING QUESTION FORM CATALOG

			// EDIT TEST
			EditTest(driver);

			// CREATE NEW QUESTION
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-outline mantine-Button-root mantine-Group-child mantine-4sv719']"))
					.click();

			// SELECT QUESTIONS
			QuestionSelectQuestions(driver);
			
			EditTestQuestions_ChooseFromCatalog(driver);
			
		} catch (Exception e) {
			System.out.println("Error = EditTestQuestion_ChooseFromCatalog");
			System.out.println(e);
		}
	}

	private static void EditTest(WebDriver driver) {
		try {
			// EDITE TEST
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

			int TestCount = driver.findElements(By.xpath("//div[@class='mantine-Col-root mantine-1akkebb']")).size()
					- 1;
			System.out.println("TEST's COUNT = " + TestCount);

			driver.findElement(By.xpath("(//button[@class='mantine-Button-light mantine-Button-root mantine-5l7wha'])["
					+ (TestCount) + "]")).click();

			driver.findElement(By.xpath(".//*[@class='icon icon-tabler icon-tabler-edit']")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestButton");
			System.out.println(e);
		}
	}

	private static void QuestionAddTitle(WebDriver driver) {
		try {
			Date date = new Date();
			String s = date.toLocaleString();
			WebElement TestTitle = driver.findElement(By.xpath("//input[@placeholder='Question Name']"));
			TestTitle.clear();
			TestTitle.sendKeys("Automation QUESTION | " + s);
			System.out.println("TITLE = 'NEW QUESTION' | " + s);

		} catch (Exception e) {
			System.out.println("Error = Question Title");
			System.out.println(e);
		}
	}

	private static void QuestionAddTags(WebDriver driver) {
		try {
			WebElement Tags = driver.findElement(By.xpath("//input[@placeholder='Select tags']"));
			Tags.sendKeys("React", Keys.ENTER);
			Tags.sendKeys("C#", Keys.ENTER);
			Tags.sendKeys("JavaScript", Keys.ENTER);
			Tags.sendKeys("Python", Keys.ENTER);
			System.out.println("TAGS = ['React','C#','JavaScript','Python']");

		} catch (Exception e) {
			System.out.println("Error = Tags");
			System.out.println(e);
		}
	}

	private static void QuestionAddDescription(WebDriver driver) {
		try {
			WebElement Description = driver.findElement(
					By.xpath("//div[@class='container']//textarea[contains(@class,'w-md-editor-text-input')]"));
			Description.click();
			Description.clear();
			Description.sendKeys("QUESTION DESCRIPTION");
			System.out.println("DESCRIPTION = 'QUESTION DESCRIPTION'");

		} catch (Exception e) {
			System.out.println("Error = Description");
			System.out.println(e);
		}
	}

	private static void QuestionAddAnswerType(WebDriver driver, int type) {
		try {
			switch (type) {
			case 1: {
				// CHOICE 'TEXT' TYPE
				driver.findElement(By.xpath("//label[contains(text(),'Text')]")).click();
				System.out.println("ANSWER TYPE = 'TEXT'");
				break;
			}
			case 2: {
				// CHOICE 'MULTIPLE' TYPE
				driver.findElement(By.xpath("//label[contains(text(),'Multiple')]")).click();
				System.out.println("ANSWER TYPE = 'TEXT'");
				break;
			}
			default:
				throw new IllegalArgumentException("Unexpected value: " + type);
			}

		} catch (Exception e) {
			System.out.println("Error = Answer Type");
			System.out.println(e);
		}
	}

	private static void QuestionAddSelectLevel(WebDriver driver) {
		try {
			// GENERATE RANDOM NUMBER FOR LEVEL
			int levels = driver.findElements(By.xpath("//input[@class='mantine-1ll72zi mantine-RadioGroup-radio']"))
					.size();
			Random rnd = new Random();
			int select = rnd.nextInt((levels - (levels - 1)), levels);

			driver.findElement(By.xpath("(//input[@class='mantine-1ll72zi mantine-RadioGroup-radio'])[" + select + "]"))
					.click();
			System.out.println("SELECT LEVEL = 'QUESTION LEVEL' (" + select + ")");

		} catch (Exception e) {
			System.out.println("Error = Select Type");
			System.out.println(e);
		}
	}

	private static void QuestionSelectQuestions(WebDriver driver) {
		int QuestionCount = driver
				.findElements(By.xpath("//div[@class='mantine-Paper-root mantine-Card-root mantine-199wbki']")).size();

		if (QuestionCount > 0) {
			for (int i = 1; i <= QuestionCount; i++) {
				driver.findElement(By.xpath(
						"(//button[@class='mantine-Button-outline mantine-Button-root mantine-da2sop'])[" + (i) + "]"))
						.click();
			}
		}

		driver.findElement(By.cssSelector(
				"button[class='mantine-Button-filled mantine-Button-root mantine-Group-child mantine-9uo5n1']"))
				.click();
	}

	private static void EditTestTitle_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));
			System.out.println("Edit Test Title = SUCCESS");
		} catch (Exception e) {
			System.out.println("Edit Test Title = FAILED");
			System.out.println(e);
		}
	}

	private static void EditTestTitle_Test1_Check(WebDriver driver) {
		// TODO NEED FIX
		try {
			Alert alert = driver.switchTo().alert();

			Assert.assertTrue(alert.getText().contains("Error"));
			System.out.println("Edit Test Title = SUCCESS");
			alert.accept();

		} catch (Exception e) {
			System.out.println("Edit Test Title - 'EMPTY' = FAILED");
			System.out.println(e);
		}
	}

	private static void EditTestDescription_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));
			System.out.println("Edit Test Description = SUCCESS");
		} catch (Exception e) {
			System.out.println("Edit Test Description = FAILED");
			System.out.println(e);
		}
	}

	private static void EditTestTags_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));
			System.out.println("Edit Test Tags = SUCCESS");
		} catch (Exception e) {
			System.out.println("Edit Test Tags = FAILED");
			System.out.println(e);
		}
	}

	private static void EditTestQuestions_CreateNewQuestion_Check(WebDriver driver) {
		try {
			int count = driver.findElements(By.cssSelector(".mantine-Paper-root.mantine-Card-root.mantine-txe1qr"))
					.size();
			Assert.assertEquals(count, 1);

			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			Thread.sleep(1000);
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));

			System.out.println("Edit Test Question - Create New Question = SUCCESS");

		} catch (Exception e) {
			System.out.println("Edit Test Question - Create New Question = FAILED");
			System.out.println(e);
		}
	}
	
	private static void EditTestQuestions_ChooseFromCatalog(WebDriver driver) {
		try {
			int count = driver.findElements(By.cssSelector(".mantine-Paper-root.mantine-Card-root.mantine-txe1qr"))
					.size();
			Assert.assertEquals(count, 1);

			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			Thread.sleep(1000);
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));

			System.out.println("Edit Test Question - Choose From Catalog = SUCCESS");

		} catch (Exception e) {
			System.out.println("Edit Test Question - Choose From Catalog = FAILED");
			System.out.println(e);
		}
	}
}
