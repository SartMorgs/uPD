Library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;


entity CPU is
    Generic (
				p_ADD_WIDTH		: INTEGER := 11;
				p_DATA_WIDTH	: INTEGER := 16
    );
    Port ( 
				i_CLK		: in  STD_LOGIC;
				i_RST		: in  STD_LOGIC;			

				o_WR_IO    	: out STD_LOGIC;
				o_RD_IO		: out STD_LOGIC;				
				i_DATA_IO	: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
				o_DATA_IO	: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
				o_ADDR_IO   : out STD_LOGIC_VECTOR((p_DATA_WIDTH-8) DOWNTO 0);

				i_INT0		: in  STD_LOGIC;
				i_INT1		: in  STD_LOGIC;
				i_INT2		: in  STD_LOGIC;
				o_BUSY      : out STD_LOGIC							
    );
end CPU;

architecture Behavioral of CPU is
-----------------------------------------------------------------

	COMPONENT PROCESSADOR is
	  Generic(
	 		p_DATA_WIDTH   : INTEGER := 16;        -- Número de bits dos dados. 
	 		p_ADD_WIDTH    : INTEGER := 9         -- Número de bits dos endereços. 
	  );
      Port ( 
				i_CLK		: in  STD_LOGIC;
				i_RST		: in  STD_LOGIC;
				
				o_EN_CLK	: out STD_LOGIC;
				o_EN_ROM	: out STD_LOGIC;
				i_DATA_ROM	: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	  
				o_ADD_ROM	: out STD_LOGIC_VECTOR(10 DOWNTO 0);
				
				o_WR_IO    	: out STD_LOGIC;
				o_RD_IO		: out STD_LOGIC;				
				i_DATA_IO	: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
				o_DATA_IO	: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
				o_ADDR_IO   : out STD_LOGIC_VECTOR((p_DATA_WIDTH-8) DOWNTO 0);

				i_INT0		: in  STD_LOGIC;
				i_INT1		: in  STD_LOGIC;
				i_INT2		: in  STD_LOGIC;
				o_BUSY      : out STD_LOGIC							
	 );
	end COMPONENT;
	
	
	COMPONENT ROM is
		Generic (
				--
				-- Size of the address bus.
				--
				p_ADD_WIDTH		: INTEGER := 11;
				p_DATA_WIDTH	: INTEGER := 16
	    );
		 Port ( 
				i_CLK	    : in STD_LOGIC;
				i_RST	    : in STD_LOGIC;
				i_EN_CLK	: in STD_LOGIC;
				i_EN 	    : in STD_LOGIC;
				i_ADDRESS   : in STD_LOGIC_VECTOR ((p_ADD_WIDTH-1) downto 0);
				o_DOUT      : out STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0)
		);
	end COMPONENT;

	
	component pll
	PORT
	(
		areset		: IN STD_LOGIC  := '0';
		inclk0		: IN STD_LOGIC  := '0';
		c0			: OUT STD_LOGIC ;
		locked		: OUT STD_LOGIC 
	);
	end component;

	
	signal 		w_ADD_ROM			: STD_LOGIC_VECTOR((p_ADD_WIDTH-1) DOWNTO 0);
	signal		w_DATA_ROM			: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal 		w_RST				: STD_LOGIC;
	signal 		w_RST_PLL			: STD_LOGIC;
	signal 		w_LOCKED			: STD_LOGIC;
	signal 		w_CLK				: STD_LOGIC;
	signal 		w_EN_CLK			: STD_LOGIC;
	signal 		w_EN_ROM			: STD_LOGIC;
	signal      w_ADD_PROCESSOR		: STD_LOGIC_VECTOR((p_ADD_WIDTH-1) DOWNTO 0);
	
-----------------------------------------------------------------	
begin

	U_PLL : pll PORT MAP (
		areset	 => w_RST_PLL,
		inclk0	 => i_CLK,
		c0	 	 => w_CLK,
		locked	 => w_LOCKED
	);
	
	--
	-- Sinal de RESET externo é invertido para uso no kit DE0.
	--
--	w_RST_PLL <= not i_RST;
	w_RST_PLL <= i_RST;
	w_RST     <= w_RST_PLL OR (NOT w_LOCKED);
	
	
	w_ADD_ROM <= w_ADD_PROCESSOR;
	
	--
	-- Instancialização do processador.
	--
	U_PROC: PROCESSADOR 
	 Generic Map(
					p_DATA_WIDTH   	=> p_DATA_WIDTH,
					p_ADD_WIDTH     => p_ADD_WIDTH
	 )
      Port Map ( 
					i_CLK			=> w_CLK,
					i_RST			=> w_RST,
					
					o_EN_CLK		=> w_EN_CLK,
					o_EN_ROM		=> w_EN_ROM,
					i_DATA_ROM		=> w_DATA_ROM,
					o_ADD_ROM		=> w_ADD_PROCESSOR,
					
					o_WR_IO    		=> o_WR_IO,
					o_RD_IO			=> o_RD_IO,				
					i_DATA_IO		=> i_DATA_IO,
					o_DATA_IO		=> o_DATA_IO,
					o_ADDR_IO		=> o_ADDR_IO,
				
					i_INT0			=> i_INT0,
					i_INT1			=> i_INT1,
					i_INT2			=> i_INT2,
					o_BUSY      	=> o_BUSY 
	 );

	 --
	 -- Instancialização da memória de programa.
	 --
	 U_MEM: ROM 
		Generic Map (
					p_ADD_WIDTH		=> p_ADD_WIDTH,
					p_DATA_WIDTH	=> p_DATA_WIDTH
	    )
		 Port Map ( 
					i_CLK			=> w_CLK,
					i_RST			=> w_RST,
					i_EN_CLK	   	=> w_EN_CLK,
					i_EN			=> w_EN_ROM,
					i_ADDRESS    	=> w_ADD_ROM,
					o_DOUT       	=> w_DATA_ROM
		); 

end behavioral;