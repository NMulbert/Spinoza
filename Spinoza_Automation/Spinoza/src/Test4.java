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

public class Test4 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Test Create Questions

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);

		driver.manage().window().maximize();
		driver.get("http://localhost:3000/");

		System.out.println("TEST START - QUESTION CREATE");
		CreateNewQuestion(driver);
		System.out.println("TEST END - QUESTION CREATE");

		Thread.sleep(5000);
		driver.quit();
	}

	public static void CreateNewQuestion(WebDriver driver) throws InterruptedException {
		driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();
		driver.findElement(By.xpath("//span[contains(text(),'NEW QUESTION')]")).click();

		// TITLE
		AddTitle(driver);

		// TAGS
		AddTags(driver);

		// DESCRIPTION
		AddDescription(driver);

		// ANSWER TYPE
		AddAnswerType(driver);

		// SELECT TYPE
		AddSelectLevel(driver);

		// SAVE
		SaveTest(driver);
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

	private static void AddAnswerType(WebDriver driver) {
		try {
			// CHOICE 'TEXT' TYPE
			driver.findElement(By.xpath("//label[contains(text(),'Text')]")).click();
			System.out.println("ANSWER TYPE = 'TEXT'");

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

	private static void SaveTest(WebDriver driver) {
		try {
			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.cssSelector("div[role='alert']")));

			String alert = driver.findElement(By.cssSelector("div[role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST FAILED");
				driver.findElement(By.cssSelector(
						"button[class='mantine-ActionIcon-hover mantine-ActionIcon-root mantine-Modal-close mantine-vao037']"))
						.click();
			} else {
				System.out.println("TEST SUCCESS");
				driver.findElement(By.cssSelector(
						"button[class='mantine-ActionIcon-hover mantine-ActionIcon-root mantine-Modal-close mantine-vao037']"))
						.click();

				// RE-ENTER TO QUESTIONS
				driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
				driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();
			}

		} catch (Exception e) {
			System.out.println("Error = Save As Draft");
			System.out.println(e);
		}

	}

}
