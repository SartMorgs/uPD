library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;


entity CAMINHO_DADOS is
    Generic (
					p_DATA_WIDTH			: INTEGER := 16
	 );
    Port ( 
				i_CLK			: in  STD_LOGIC;
				i_RST			: in  STD_LOGIC;
				i_DATA			: in  STD_LOGIC_VECTOR(10 DOWNTO 0);	  
				i_SEL_ULA		: in  STD_LOGIC_VECTOR(2 DOWNTO 0);	
				i_SEL_IMED		: in  STD_LOGIC_VECTOR(1 DOWNTO 0);	
			    i_WR_BCO		: in  STD_LOGIC;
				i_WR_RAM    	: in  STD_LOGIC;
				i_WR_IO       	: in  STD_LOGIC;
				i_RD_IO       	: in  STD_LOGIC;
				i_DATA_IO		: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
				o_DATA_ULA		: out STD_LOGIC;
				o_DATA_IO		: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
				o_ADDR_IO      	: out STD_LOGIC_VECTOR((p_DATA_WIDTH-8) DOWNTO 0);
				i_RET			: in  STD_LOGIC;
				i_DOUT_LIFO 	: in  STD_LOGIC_VECTOR(63 DOWNTO 0);
				o_DIN_LIFO  	: out STD_LOGIC_VECTOR(63 DOWNTO 0)					
	 );
end CAMINHO_DADOS;

architecture Behavioral of CAMINHO_DADOS is
-----------------------------------------------------------------
	COMPONENT BANCO_REGISTRADOR is
	    Generic (
					p_DATA_WIDTH    : INTEGER := 16
	    );
		 Port ( 
					i_CLK   		: in  STD_LOGIC;
					i_RST   		: in  STD_LOGIC;
					
					i_ADD_S1		: in  STD_LOGIC_VECTOR(1 DOWNTO 0);	  
					i_ADD_S2		: in  STD_LOGIC_VECTOR(1 DOWNTO 0);	

					i_RET			: in  STD_LOGIC;
					i_DOUT_LIFO 	: in  STD_LOGIC_VECTOR(63 DOWNTO 0);
					o_DIN_LIFO  	: out STD_LOGIC_VECTOR(63 DOWNTO 0);		
				
					i_WR_IO       	: in  STD_LOGIC;
					i_WR_RAM 		: in  STD_LOGIC;
					i_WR			: in  STD_LOGIC;
					i_DATA			: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
					i_ADD_DST 		: in  STD_LOGIC_VECTOR(1 DOWNTO 0);
					o_RS1 			: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
					o_RS2 			: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0)
		 );
	end COMPONENT;

	component ULA is
	 Generic(
				p_DATA_WIDTH    : INTEGER := 16
	 );
    Port ( 
				i_REG_S1		: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	  
				i_REG_S2		: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
			     i_SEL_ULA  	: in  STD_LOGIC_VECTOR(2 DOWNTO 0);	
				o_DATA_ULA		: out  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0)
	 );
	end component;

	COMPONENT REGISTRADOR is
		 Generic (
					p_DATA_WIDTH	: INTEGER := 16
		 );
		 Port ( 
					i_CLK   	: in  STD_LOGIC;
					i_RST   	: in  STD_LOGIC;
					i_DATA		: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	  
					o_DATA		: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
					i_WR   		: in  STD_LOGIC	
		 );
	end COMPONENT;	

	COMPONENT MEMORIA is
		 Generic(
					 p_DATA_WIDTH   : INTEGER := 16;        -- Número de bits dos dados. 
					 p_ADD_WIDTH    : INTEGER := 6         -- Número de bits dos endereços. 
		 );
		 Port ( 
				  i_CLK 	: in  STD_LOGIC;
				  i_RST  	: in  STD_LOGIC;
				  i_WR 		: in  STD_LOGIC;
				  i_ADDR  	: in  STD_LOGIC_VECTOR ((p_ADD_WIDTH-1) downto 0);
				  i_DATA  	: in  STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0);
				  o_DATA  	: out STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0)
		 );
	end COMPONENT;

	
	signal   w_RS1			: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_RS2			: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_DATA_ULA		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_DATA_BCO		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_DATA_IO		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_DATA_RAM		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_EN_ADD_IO	: STD_LOGIC;
	signal   w_CLK			: STD_LOGIC;
	
-----------------------------------------------------------------	
begin
	

	MUX : process(i_SEL_iMED, i_DATA(5 downto 0), w_DATA_IO, w_DATA_ULA, w_DATA_RAM)
	begin
		if (i_SEL_iMED = "00") then
				w_DATA_BCO <= "0000000" & i_DATA((p_DATA_WIDTH-8) downto 0);	
		elsif (i_SEL_iMED = "01") then
				w_DATA_BCO <= w_DATA_IO;	
		elsif (i_SEL_iMED = "10") then
				w_DATA_BCO <= w_DATA_RAM;	
		else
				w_DATA_BCO <= w_DATA_ULA;
		end if;
	end process MUX;
	
	
	U01 : BANCO_REGISTRADOR 
	    Generic Map (
					p_DATA_WIDTH    => p_DATA_WIDTH
	    )
		 Port Map ( 
						i_CLK   		=> i_CLK,
						i_RST   		=> i_RST,
						i_ADD_S1		=> i_DATA((p_DATA_WIDTH-8) downto (p_DATA_WIDTH-9)),
						i_ADD_S2		=> i_DATA((p_DATA_WIDTH-10) downto (p_DATA_WIDTH-11)),				
						i_RET			=> i_RET,
						i_DOUT_LIFO 	=> i_DOUT_LIFO,
						o_DIN_LIFO  	=> o_DIN_LIFO,								
						i_WR_IO			=> i_WR_IO,
						i_WR_RAM 		=> i_WR_RAM,
						i_WR  			=> i_WR_BCO,
						i_DATA			=> w_DATA_BCO,
						i_ADD_DST 		=> i_DATA((p_DATA_WIDTH-6) downto (p_DATA_WIDTH-7)),
						o_RS1 			=> w_RS1,
						o_RS2 			=> w_RS2
		 );
	
	U02 : ULA 
	 Generic Map (
					p_DATA_WIDTH    => p_DATA_WIDTH
	 )
      Port Map( 
					i_REG_S1		=> w_RS1,  
					i_REG_S2		=> w_RS2,
					i_SEL_ULA   	=> i_SEL_ULA,
					o_DATA_ULA	    => w_DATA_ULA
	 );

	--
	-- Usado nas instruções de jump (JE e JZ).
	--
	o_DATA_ULA <= w_DATA_ULA(0);
	
	
	
	--
	-- Registrado de ENDERECO de (IO).
	--
	U03A:  REGISTRADOR 
	  Generic Map (
					p_DATA_WIDTH => 9
	  )
      Port map( 
					i_CLK   	=> i_CLK,
					i_RST   	=> i_RST,
					i_DATA		=> i_DATA((p_DATA_WIDTH-8) downto 0),
					o_DATA		=> o_ADDR_IO,
					i_WR   		=> w_EN_ADD_IO
	 );
	 
	 w_EN_ADD_IO <= i_WR_IO OR i_RD_IO; 
	 
	 
	--
	-- Registrado de saída de dados (IO).
	--
	U03:  REGISTRADOR 
	 Generic Map (
					p_DATA_WIDTH => p_DATA_WIDTH
	 )	
     Port map( 
					i_CLK   	=> i_CLK,
					i_RST   	=> i_RST,
					i_DATA		=> w_RS1,
					o_DATA		=> o_DATA_IO,
					i_WR   		=> i_WR_IO
	 );

	--
	-- Registrado de entrada de dados (IO).
	--
	U04:  REGISTRADOR 
	Generic Map (
					p_DATA_WIDTH => p_DATA_WIDTH
	)	
	Port map( 
					i_CLK   	=> i_CLK,
					i_RST   	=> i_RST,
					i_DATA		=> I_DATA_IO,
					o_DATA		=> w_DATA_IO,
					i_WR   		=> i_RD_IO
	 );
	 
	 
	 w_CLK <= i_CLK;
	 
	 U05 : MEMORIA 
		Generic Map(
						 p_DATA_WIDTH   => p_DATA_WIDTH,
						 p_ADD_WIDTH    => 9        
		)
		Port Map( 
					  i_CLK 	=> w_CLK,
					  i_RST   	=> i_RST,
					  i_WR 		=> i_WR_RAM,
					  i_ADDR 	=> i_DATA((p_DATA_WIDTH-8) downto 0),
					  i_DATA 	=> w_RS1,
					  o_DATA 	=> w_DATA_RAM
	 );
	 
end behavioral;