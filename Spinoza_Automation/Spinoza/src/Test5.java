import java.util.Date;
import java.util.Random;
import java.util.concurrent.TimeUnit;
import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;

public class Test5 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Tests Create Question

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);

		driver.manage().window().maximize();
//		driver.get("http://localhost:3000/");
		driver.get("https://www.zionetapp.com/");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST START - QUESTION'S CREATE");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 1");
		CreateNewQuestion(driver, 2); // CREATE QUESTION WITH 'MULTIPLE' ANSWERS
		System.out.println("TEST END - TEST 1");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 2");
		CreateNewQuestion(driver, 1); // CREATE QUESTION WITH 'TEXT' ANSWERS
		System.out.println("TEST END - TEST 2");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST END - QUESTION'S CREATE");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		Thread.sleep(5000);
		driver.quit();

	}

	public static void CreateNewQuestion(WebDriver driver, int type) throws InterruptedException {
		try {
			// CREATE QUESTION WITH SPECIFIC TYPE

			driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();
			Thread.sleep(5000);
			driver.findElement(By.xpath("//span[contains(text(),'NEW QUESTION')]")).click();

			// TITLE
			AddTitle(driver);

			// TAGS
			AddTags(driver);

			// DESCRIPTION
			AddDescription(driver);

			// ANSWER TYPE
			switch (type) {
			case 1: {
				AddAnswerType(driver, type);
				break;
			}
			case 2: {
				AddAnswerType(driver, type);
				MultipleChoice(driver, type, 1);
				break;
			}
			default:
				throw new IllegalArgumentException("Unexpected value: " + type);
			}

			// SELECT TYPE
			AddSelectLevel(driver);

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.cssSelector("div[role='alert']")));

			CreateNewQuestion_Check(driver);

			driver.findElement(By.cssSelector(
					"button[class='mantine-ActionIcon-hover mantine-ActionIcon-root mantine-Modal-close mantine-vao037']"))
					.click();
			Thread.sleep(5000);
			// RE-ENTER TO QUESTIONS
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
			driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();

		} catch (Exception e) {
			System.out.println("Error = CreateNewQuestion_Test" + type);
			System.out.println(e);
		}
	}

	private static void AddTitle(WebDriver driver) {
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

	private static void AddTags(WebDriver driver) {
		try {
			WebElement Tags = driver.findElement(By.xpath("//input[@placeholder='Select tags']"));
//			Tags.click();
			Tags.sendKeys("React", Keys.ENTER);
			Tags.sendKeys("C#", Keys.ENTER);
			Tags.sendKeys("JavaScript", Keys.ENTER);
			Tags.sendKeys("Python", Keys.ENTER);
			Tags.click();
			System.out.println("TAGS = ['React','C#','JavaScript','Python']");

		} catch (Exception e) {
			System.out.println("Error = Tags");
			System.out.println(e);
		}
	}

	private static void AddDescription(WebDriver driver) {
		try {
			WebElement Description = driver.findElement(By.cssSelector(".w-md-editor-text-input"));
			Description.click();
			Description.clear();
			Description.sendKeys("QUESTION DESCRIPTION");
			System.out.println("DESCRIPTION = 'QUESTION DESCRIPTION'");

		} catch (Exception e) {
			System.out.println("Error = Description");
			System.out.println(e);
		}
	}

	private static void AddAnswerType(WebDriver driver, int type) {
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

	private static void AddSelectLevel(WebDriver driver) {
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

	private static void MultipleChoice(WebDriver driver, int choices, int correct) {
		try {
			// ADD OPTTIONS
			for (int i = 1; i <= choices; i++) {
				driver.findElement(By.xpath("//span[contains(text(),'+ Add option')]")).click();

				driver.findElement(By.xpath("(//textarea[@placeholder='Add option'])[" + i + "]"))
						.sendKeys("QUESTION OPTION " + i);
			}

			// SET CORRECT ANSWER
			driver.findElement(By.xpath("//span[contains(text(),'Correct answer')]")).click();

			driver.findElement(By.xpath("(//input[@name='Answer'])[" + correct + "]")).click();
			
			driver.findElement(By.xpath("//span[contains(text(),'Done')]")).click();

		} catch (Exception e) {
			System.out.println("Error = Multiple Choice");
			System.out.println(e);
		}

	}

	private static void CreateNewQuestion_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("created"));
			System.out.println("Create Test2 = SUCCESS");
		} catch (Exception e) {
			System.out.println("Create Test2 = FAILED");
			System.out.println(e);
		}
	}
}
